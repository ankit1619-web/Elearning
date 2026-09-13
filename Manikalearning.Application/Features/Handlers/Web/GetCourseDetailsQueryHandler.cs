using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Web
{
    public class GetCourseDetailsQueryHandler : IRequestHandler<GetCourseDetailsQuery, CourseDto>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<CourseSection> _courseSectionRepository;
        private readonly IRepository<CourseLecture> _courseLectureRepository;

        public GetCourseDetailsQueryHandler(
            IRepository<Course> courseRepository,
            IRepository<CourseSection> courseSectionRepository,
            IRepository<CourseLecture> courseLectureRepository)
        {
            _courseRepository = courseRepository;
            _courseSectionRepository = courseSectionRepository;
            _courseLectureRepository = courseLectureRepository;
        }

        public async Task<CourseDto> Handle(GetCourseDetailsQuery request, CancellationToken cancellationToken)
        {
            var course = await _courseRepository.Entity()
                .Where(c => c.CourseId == request.CourseId)
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    CourseName = c.CourseName,
                    Description = c.Description,
                    Price = c.Price
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {request.CourseId} was not found.");
            }
             
            var sections = await _courseSectionRepository.Entity()
                .Where(s => s.CourseId == request.CourseId && s.IsActive)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync(cancellationToken);

            var sectionIds = sections.Select(s => s.CourseSectionId).ToList();
             
            var lectures = await _courseLectureRepository.Entity()
                .Where(l => sectionIds.Contains(l.CourseSectionId) && l.IsPublished)
                .OrderBy(l => l.DisplayOrder)
                .ToListAsync(cancellationToken);
             
            course.Sections = sections.Select(s => new CourseSectionDto
            {
                CourseSectionId = s.CourseSectionId,
                CourseId = s.CourseId,
                SectionName = s.SectionName,
                Description = s.Description,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive,
                Lectures = lectures
                    .Where(l => l.CourseSectionId == s.CourseSectionId)
                    .Select(l => new CourseLectureDto
                    {
                        LectureId = l.LectureId,
                        CourseSectionId = l.CourseSectionId,
                        LectureTitle = l.LectureTitle,
                        Description = l.Description,
                        VideoUrl = l.VideoUrl,
                        VideoDurationSeconds = l.VideoDurationSeconds,
                        NotesUrl = l.NotesUrl,
                        IsFreePreview = l.IsFreePreview,
                        DisplayOrder = l.DisplayOrder,
                        IsPublished = l.IsPublished,
                        CreatedAt = l.CreatedAt
                    }).ToList()
            }).ToList();

            course.TotalLectures = lectures.Count;

            return course;
        }
    }
} 
