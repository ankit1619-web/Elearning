using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Application.Services;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record GetPlanQuery(string? CouponCode = null) : IRequest<PlansResultDto>;

    public class GetPlansQueryValidator : AbstractValidator<GetPlanQuery>
    {
        public GetPlansQueryValidator()
        {
            RuleFor(x => x.CouponCode).MaximumLength(50)
                .Matches("^[A-Za-z0-9_-]*$").WithMessage("Coupon code can only have letters, numbers, - and _.");
        }
    }
    public class GetPlansQueryHandler : IRequestHandler<GetPlanQuery, PlansResultDto>
    {
        private readonly IRepository<Plan> _plans;
        private readonly IRepository<PlanCourse> _planCourses;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<Coupon> _coupons;

        public GetPlansQueryHandler(IRepository<Plan> plans, IRepository<PlanCourse> planCourses, IRepository<Course> courses, IRepository<Coupon> coupons)
        { _plans = plans; _planCourses = planCourses; _courses = courses; _coupons = coupons; }

        public async Task<PlansResultDto> Handle(GetPlanQuery req, CancellationToken ct)
        {
            var plans = await _plans.Entity().AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.Price).ToListAsync(ct);
            var ids = plans.Select(p => p.PlanId).ToList();

            var courseRows = await (from pc in _planCourses.Entity().AsNoTracking()
                                    join c in _courses.Entity().AsNoTracking() on pc.CourseId equals c.CourseId
                                    where ids.Contains(pc.PlanId) && c.IsPublished
                                    orderby c.CourseName
                                    select new { pc.PlanId, c.CourseName }).ToListAsync(ct);
            var coursesByPlan = courseRows.GroupBy(x => x.PlanId).ToDictionary(g => g.Key, g => g.Select(x => x.CourseName).ToList());

            var code = req.CouponCode?.Trim();
            Coupon? coupon = null;
            var result = new PlansResultDto { CouponCode = code };

            if (!string.IsNullOrEmpty(code))
            {
                coupon = await _coupons.Entity().AsNoTracking().FirstOrDefaultAsync(c => c.CouponCode == code, ct);
                var usable = coupon != null && CouponCalculator.IsUsable(coupon, DateTime.UtcNow);
                result.CouponValid = usable;
                result.CouponMessage = usable ? null : "This coupon is not valid.";
                if (!usable) coupon = null;
            }

            result.Plans = plans.Select(p =>
            {
                var discount = coupon != null ? CouponCalculator.Discount(coupon, p.Price) : 0;
                return new PlanDto
                {
                    PlanId = p.PlanId,
                    PlanName = p.PlanName,
                    Description = p.Description,
                    DurationDays = p.DurationDays,
                    Price = p.Price,
                    Discount = discount,
                    FinalPrice = p.Price - discount,
                    Courses = coursesByPlan.TryGetValue(p.PlanId, out var list) ? list : new List<string>()
                };
            }).ToList();

            if (coupon != null && result.Plans.All(p => p.Discount == 0))
            {
                result.CouponValid = false;
                result.CouponMessage = "This coupon does not apply to these plans (minimum amount not reached).";
            }
            else if (coupon != null)
            {
                result.CouponMessage = "Coupon applied. Prices below include your discount.";
            }
            return result;
        }
    }
}
