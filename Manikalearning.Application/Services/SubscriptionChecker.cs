using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Services
{
    public class SubscriptionChecker : ISubscriptionChecker
    {
        private readonly IRepository<StudentSubscription> _subs;
        private readonly IRepository<PlanCourse> _planCourses;
        private readonly IRepository<Course> _courses;

        public SubscriptionChecker(IRepository<StudentSubscription> subs, IRepository<PlanCourse> planCourses, IRepository<Course> courses)
        { _subs = subs; _planCourses = planCourses; _courses = courses; }
         
        private IQueryable<StudentSubscription> ActiveFor(Guid userId, DateTime now) =>
            _subs.Entity().AsNoTracking().Where(s => s.UserId == userId && s.Status == SubStatus.Active && s.StartDate <= now && s.EndDate >= now);

        public Task<bool> HasActiveAsync(Guid userId, CancellationToken ct = default) =>
            ActiveFor(userId, DateTime.UtcNow).AnyAsync(ct);

        public async Task<bool> CanAccessCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default)
        {
            var course = await _courses.Entity().AsNoTracking()
                .Where(c => c.CourseId == courseId && c.IsPublished)
                .Select(c => new { c.IsFree }).FirstOrDefaultAsync(ct);
            if (course == null) return false;                
            if (course.IsFree) return true;

            return await (from s in ActiveFor(userId, DateTime.UtcNow)
                          join pc in _planCourses.Entity().AsNoTracking() on s.PlanId equals pc.PlanId
                          where pc.CourseId == courseId
                          select 1).AnyAsync(ct);
        }

        public async Task<HashSet<Guid>> AccessibleCourseIdsAsync(Guid userId, CancellationToken ct = default)
        {
            var free = await _courses.Entity().AsNoTracking().Where(c => c.IsPublished && c.IsFree).Select(c => c.CourseId).ToListAsync(ct);

            var paid = await (from s in ActiveFor(userId, DateTime.UtcNow)
                              join pc in _planCourses.Entity().AsNoTracking() on s.PlanId equals pc.PlanId
                              join c in _courses.Entity().AsNoTracking() on pc.CourseId equals c.CourseId
                              where c.IsPublished
                              select c.CourseId).Distinct().ToListAsync(ct);

            return free.Concat(paid).ToHashSet();
        }
    }
}
