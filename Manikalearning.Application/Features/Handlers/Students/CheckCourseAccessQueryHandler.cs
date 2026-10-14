using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record CheckCourseAccessQuery(Guid UserId, Guid CourseId) : IRequest<CourseAccessDto>;

    public class CheckCourseAccessQueryValidator : AbstractValidator<CheckCourseAccessQuery>
    {
        public CheckCourseAccessQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CourseId).NotEmpty();
        }
    }

    public class CheckCourseAccessQueryHandler : IRequestHandler<CheckCourseAccessQuery, CourseAccessDto>
    {
        private readonly ISubscriptionChecker _checker;
        private readonly IRepository<Course> _courses;
        public CheckCourseAccessQueryHandler(ISubscriptionChecker checker, IRepository<Course> courses) { _checker = checker; _courses = courses; }

        public async Task<CourseAccessDto> Handle(CheckCourseAccessQuery req, CancellationToken ct)
        {
            var course = await _courses.Entity().AsNoTracking()
                .Where(c => c.CourseId == req.CourseId && c.IsPublished).Select(c => new { c.IsFree }).FirstOrDefaultAsync(ct);
            if (course == null) return new CourseAccessDto { HasAccess = false, Reason = "NotFound" };
            if (course.IsFree) return new CourseAccessDto { HasAccess = true, Reason = "Free" };

            var ok = await _checker.CanAccessCourseAsync(req.UserId, req.CourseId, ct);
            return new CourseAccessDto { HasAccess = ok, Reason = ok ? "Subscription" : "NoSubscription" };
        }
    }
}
