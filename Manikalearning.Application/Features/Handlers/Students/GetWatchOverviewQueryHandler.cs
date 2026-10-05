using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetWatchOverviewQueryHandler : IRequestHandler<GetWatchOverviewQuery, WatchOverviewViewModel>
    {
        private readonly IRepository<Course> _courseRepository;

        public GetWatchOverviewQueryHandler(IRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<WatchOverviewViewModel> Handle(GetWatchOverviewQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.Query()
                .AsNoTracking()
                .Include(c => c.CourseSections)
                    .ThenInclude(cs => cs.CourseLectures)
                .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {request.CourseId} was not found.");
            }

            var totalLectures = course.CourseSections.SelectMany(cs => cs.CourseLectures).Count();

            return new WatchOverviewViewModel
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseName,
                CourseDescription = course.Description,
                TotalModules = course.CourseSections.Count,
                TotalLessons = totalLectures,
                Modules = course.CourseSections.Select(cs => new ModuleDto
                {
                    ModuleId = cs.CourseSectionId,
                    Title = cs.SectionName,
                    Lessons = cs.CourseLectures.OrderBy(l => l.DisplayOrder).Select(l => new LessonDto
                    {
                        LessonId = l.LectureId,
                        Title = l.LectureTitle,
                        Duration = l.VideoDurationSeconds.ToString()!, 
                        IsFree = l.IsFreePreview
                    }).ToList()
                }).ToList()
            };
        }

    }
}