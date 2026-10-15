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
        private readonly IRepository<StudentCourse> _enrollments;
        private readonly IRepository<CourseLecture> _lectures;      
        private readonly IRepository<CourseSection> _sections;  

        private readonly IRepository<PlanFeature> _planFeatures;

        public SubscriptionChecker(IRepository<StudentSubscription> subs, IRepository<PlanCourse> planCourses, IRepository<Course> courses,
                                   IRepository<StudentCourse> enrollments, IRepository<CourseLecture> lectures, IRepository<CourseSection> sections,
                                   IRepository<PlanFeature> planFeatures)
        { _subs = subs; _planCourses = planCourses; _courses = courses; _enrollments = enrollments; _lectures = lectures; _sections = sections; _planFeatures = planFeatures; }

        
        private IQueryable<StudentSubscription> ActiveSubs(Guid userId, DateTime now) =>
            _subs.Entity().AsNoTracking().Where(s => s.UserId == userId && s.Status == SubStatus.Active && s.StartDate <= now && s.EndDate >= now);

        private IQueryable<StudentCourse> ActiveEnrollments(Guid userId, DateTime now) =>
            _enrollments.Entity().AsNoTracking().Where(e => e.UserId == userId && e.IsActive && e.StartDate <= now && (e.ExpiryDate == null || e.ExpiryDate >= now));

        public Task<bool> HasActiveAsync(Guid userId, CancellationToken ct = default) =>
            ActiveSubs(userId, DateTime.UtcNow).AnyAsync(ct);

        public async Task<CourseAccessDto> CheckCourseAsync(Guid userId, Guid courseId, CancellationToken ct = default)
        {
            var course = await _courses.Entity().AsNoTracking()
                .Where(c => c.CourseId == courseId && c.IsPublished).Select(c => new { c.IsFree }).FirstOrDefaultAsync(ct);
            if (course == null) return new CourseAccessDto { HasAccess = false, Reason = "NotFound", CourseId = courseId };
            if (course.IsFree) return new CourseAccessDto { HasAccess = true, Reason = "Free", CourseId = courseId };

            var now = DateTime.UtcNow;

            var viaPlan = await (from s in ActiveSubs(userId, now)
                                 join pc in _planCourses.Entity().AsNoTracking() on s.PlanId equals pc.PlanId
                                 where pc.CourseId == courseId
                                 select 1).AnyAsync(ct);
            if (viaPlan) return new CourseAccessDto { HasAccess = true, Reason = "Subscription", CourseId = courseId };

            var enrolled = await ActiveEnrollments(userId, now).AnyAsync(e => e.CourseId == courseId, ct);
            return enrolled
                ? new CourseAccessDto { HasAccess = true, Reason = "Enrolled", CourseId = courseId }
                : new CourseAccessDto { HasAccess = false, Reason = "NoSubscription", CourseId = courseId };
        }

        public async Task<CourseAccessDto> CheckLectureAsync(Guid userId, Guid lectureId, CancellationToken ct = default)
        { 
            var row = await (from l in _lectures.Entity().AsNoTracking()
                             join s in _sections.Entity().AsNoTracking() on l.CourseSectionId equals s.CourseSectionId
                             where l.LectureId == lectureId && l.IsPublished && s.IsActive
                             select new { s.CourseId, l.IsFreePreview }).FirstOrDefaultAsync(ct);
            if (row == null) return new CourseAccessDto { HasAccess = false, Reason = "NotFound" };

            var course = await CheckCourseAsync(userId, row.CourseId, ct);
            if (course.HasAccess) return course;
             
            if (row.IsFreePreview && course.Reason != "NotFound")
                return new CourseAccessDto { HasAccess = true, Reason = "FreePreview", CourseId = row.CourseId };

            return course;
        }
         
        public async Task<CourseAccessDto> CheckMediaAsync(Guid userId, string fileName, CancellationToken ct = default)
        {
            var lectureIds = await _lectures.Entity().AsNoTracking()
                .Where(l => l.IsPublished &&
                            ((l.VideoUrl != null && l.VideoUrl.EndsWith(fileName)) || (l.NotesUrl != null && l.NotesUrl.EndsWith(fileName))))
                .Select(l => l.LectureId).Take(20).ToListAsync(ct);

            if (lectureIds.Count == 0) return new CourseAccessDto { HasAccess = true, Reason = "NotCourseMedia" };

            foreach (var id in lectureIds)
            {
                var r = await CheckLectureAsync(userId, id, ct);
                if (r.HasAccess) return r;
            }
            return new CourseAccessDto { HasAccess = false, Reason = "NoSubscription" };
        }
         
        public async Task<HashSet<string>> GetFeaturesAsync(Guid userId, CancellationToken ct = default)
        {
            var codes = await (from s in ActiveSubs(userId, DateTime.UtcNow)
                               join f in _planFeatures.Entity().AsNoTracking() on s.PlanId equals f.PlanId
                               select f.FeatureCode).Distinct().ToListAsync(ct);
            return codes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public Task<bool> HasFeatureAsync(Guid userId, string feature, CancellationToken ct = default) =>
            (from s in ActiveSubs(userId, DateTime.UtcNow)
             join f in _planFeatures.Entity().AsNoTracking() on s.PlanId equals f.PlanId
             where f.FeatureCode == feature
             select 1).AnyAsync(ct);

        public async Task<HashSet<Guid>> AccessibleCourseIdsAsync(Guid userId, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var free = await _courses.Entity().AsNoTracking().Where(c => c.IsPublished && c.IsFree).Select(c => c.CourseId).ToListAsync(ct);

            var viaPlan = await (from s in ActiveSubs(userId, now)
                                 join pc in _planCourses.Entity().AsNoTracking() on s.PlanId equals pc.PlanId
                                 join c in _courses.Entity().AsNoTracking() on pc.CourseId equals c.CourseId
                                 where c.IsPublished
                                 select c.CourseId).Distinct().ToListAsync(ct);

            var enrolled = await (from e in ActiveEnrollments(userId, now)
                                  join c in _courses.Entity().AsNoTracking() on e.CourseId equals c.CourseId
                                  where c.IsPublished
                                  select c.CourseId).Distinct().ToListAsync(ct);

            return free.Concat(viaPlan).Concat(enrolled).ToHashSet();
        }
    }
}
