using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Application.Services;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class PlanCourseOptionDto
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public int PlanCount { get; set; }
        public decimal MinPrice { get; set; }
    }
     
    public record GetPlanCoursesQuery() : IRequest<List<PlanCourseOptionDto>>;

    public class GetPlanCoursesQueryHandler : IRequestHandler<GetPlanCoursesQuery, List<PlanCourseOptionDto>>
    {
        private readonly IRepository<Plan> _plans;
        private readonly IRepository<PlanCourse> _planCourses;
        private readonly IRepository<Course> _courses;

        public GetPlanCoursesQueryHandler(IRepository<Plan> plans, IRepository<PlanCourse> planCourses, IRepository<Course> courses)
        { _plans = plans; _planCourses = planCourses; _courses = courses; }

        public async Task<List<PlanCourseOptionDto>> Handle(GetPlanCoursesQuery req, CancellationToken ct)
        {
            var rows = await (from p in _plans.Entity().AsNoTracking()
                              join pc in _planCourses.Entity().AsNoTracking() on p.PlanId equals pc.PlanId
                              join c in _courses.Entity().AsNoTracking() on pc.CourseId equals c.CourseId
                              where p.IsActive && c.IsPublished
                              select new { c.CourseId, c.CourseName, p.PlanId, p.Price }).ToListAsync(ct);

            return rows.GroupBy(x => new { x.CourseId, x.CourseName })
                .Select(g => new PlanCourseOptionDto
                {
                    CourseId = g.Key.CourseId,
                    CourseName = g.Key.CourseName,
                    PlanCount = g.Select(x => x.PlanId).Distinct().Count(),
                    MinPrice = g.Min(x => x.Price)
                })
                .OrderBy(x => x.CourseName).ToList();
        }
    }
     
    public record GetPlanQuery(string? CouponCode = null, Guid? CourseId = null) : IRequest<PlansResultDto>;

    public class GetPlanQueryValidator : AbstractValidator<GetPlanQuery>
    {
        public GetPlanQueryValidator()
        {
            RuleFor(x => x.CouponCode).MaximumLength(50)
                .Matches("^[A-Za-z0-9_-]*$").WithMessage("Coupon code can only have letters, numbers, - and _.");
        }
    }
    public class GetPlanQueryHandler : IRequestHandler<GetPlanQuery, PlansResultDto>
    {
        private readonly IRepository<Plan> _plans;
        private readonly IRepository<PlanCourse> _planCourses;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<Coupon> _coupons;
        private readonly IRepository<PlanFeature> _planFeatures;

        public GetPlanQueryHandler(IRepository<Plan> plans, IRepository<PlanCourse> planCourses, IRepository<Course> courses,
                                    IRepository<Coupon> coupons, IRepository<PlanFeature> planFeatures)
        { _plans = plans; _planCourses = planCourses; _courses = courses; _coupons = coupons; _planFeatures = planFeatures; }

        public async Task<PlansResultDto> Handle(GetPlanQuery req, CancellationToken ct)
        {
            var plansQuery = _plans.Entity().AsNoTracking().Where(p => p.IsActive);
            if (req.CourseId != null)
            {
                var planIdsForCourse = await _planCourses.Entity().AsNoTracking()
                    .Where(pc => pc.CourseId == req.CourseId).Select(pc => pc.PlanId).ToListAsync(ct);
                plansQuery = plansQuery.Where(p => planIdsForCourse.Contains(p.PlanId));
            }
            var plans = await plansQuery.OrderBy(p => p.Price).ToListAsync(ct);
            var ids = plans.Select(p => p.PlanId).ToList();

            var featureRows = await _planFeatures.Entity().AsNoTracking().Where(f => ids.Contains(f.PlanId))
                .Select(f => new { f.PlanId, f.FeatureCode }).ToListAsync(ct);
            var featuresByPlan = featureRows.GroupBy(x => x.PlanId)
                .ToDictionary(g => g.Key, g => g.Select(x => PlanFeatureCodes.Label(x.FeatureCode)).ToList());

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
                    //Courses = coursesByPlan.TryGetValue(p.PlanId, out var list) ? list : new List<string>(),
                    Features = featuresByPlan.TryGetValue(p.PlanId, out var fl) ? fl : new List<string>()
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
 
