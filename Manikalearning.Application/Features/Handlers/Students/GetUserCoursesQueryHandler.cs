using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetUserCoursesQueryHandler : IRequestHandler<GetUserCoursesQuery, MyCoursesViewModel>
    {
        private readonly IRepository<Course> _courseRepo;

        public GetUserCoursesQueryHandler(IRepository<Course> courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task<MyCoursesViewModel> Handle(GetUserCoursesQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var query = _courseRepo.Query()
                .Where(c => c.StudentCourses.Any(sc =>
                    sc.UserId == request.StudentId &&
                    sc.IsActive &&
                    (sc.ExpiryDate == null || sc.ExpiryDate > now))); 
             
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.CourseName.ToLower().Contains(term));
            }
             
            var rawCourses = await query.Select(c => new
            {
                CourseId = c.CourseId,
                Title = c.CourseName,
                ThumbnailUrl = "/images/course-default.jpg",
                 
                TotalSections = c.CourseSections.Count(cs => cs.IsActive),
                TotalLectures = c.CourseSections
                    .Where(cs => cs.IsActive)
                    .SelectMany(cs => cs.CourseLectures)
                    .Count(cl => cl.IsPublished),
                     
                CompletedLectures = c.CourseSections
                    .Where(cs => cs.IsActive)
                    .SelectMany(cs => cs.CourseLectures)
                    .Where(cl => cl.IsPublished)
                    .Count(cl => cl.StudentLectureProgresses
                        .Any(slp => slp.UserId == request.StudentId && slp.IsCompleted)),
                         
                EnrolledDate = c.StudentCourses
                    .Where(sc => sc.UserId == request.StudentId)
                    .Select(sc => (DateTime?)sc.EnrolledAt)
                    .FirstOrDefault(),
                     
                LastAccessedDate = c.CourseSections
                    .SelectMany(cs => cs.CourseLectures)
                    .SelectMany(cl => cl.StudentLectureProgresses)
                    .Where(slp => slp.UserId == request.StudentId)
                    .Max(slp => (DateTime?)slp.LastWatchedAt),
                     
                NextLecture = c.CourseSections
                    .Where(cs => cs.IsActive)
                    .OrderBy(cs => cs.DisplayOrder)
                    .SelectMany(cs => cs.CourseLectures)
                    .Where(cl => cl.IsPublished && !cl.StudentLectureProgresses
                        .Any(slp => slp.UserId == request.StudentId && slp.IsCompleted))
                    .OrderBy(cl => cl.DisplayOrder)
                    .Select(cl => new { Id = (Guid?)cl.LectureId, Title = cl.LectureTitle })
                    .FirstOrDefault()
            }).ToListAsync(cancellationToken);
             
            var coursesList = rawCourses.Select(c =>
            {
                int total = c.TotalLectures;
                int completed = c.CompletedLectures;
                int percentage = total > 0 ? (int)Math.Round((double)completed / total * 100) : 0;
                bool isCompleted = total > 0 && completed >= total;

                return new EnrolledCourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    ThumbnailUrl = c.ThumbnailUrl,
                    TotalModules = c.TotalSections,
                    TotalLessons = total,
                    CompletedLessons = completed,
                    ProgressPercentage = percentage,
                    IsCompleted = isCompleted,
                    EnrolledDate = c.EnrolledDate ?? DateTime.UtcNow,
                    LastAccessedDate = c.LastAccessedDate,
                    NextLessonId = c.NextLecture?.Id,
                    NextLessonTitle = c.NextLecture?.Title
                };
            }).ToList();
             
            var filteredCourses = request.Filter?.ToLower() switch
            {
                "inprogress" => coursesList.Where(c => !c.IsCompleted).ToList(),
                "completed" => coursesList.Where(c => c.IsCompleted).ToList(),
                _ => coursesList
            };
             
            return new MyCoursesViewModel
            {
                Courses = filteredCourses,
                TotalEnrolled = coursesList.Count,
                InProgressCount = coursesList.Count(c => !c.IsCompleted),
                CompletedCount = coursesList.Count(c => c.IsCompleted),
                SelectedFilter = string.IsNullOrWhiteSpace(request.Filter) ? "All" : request.Filter,
                SearchQuery = request.SearchTerm ?? string.Empty
            };
        }
    }
}
