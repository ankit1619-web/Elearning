using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Web
{ 
    public class GetCoursesPageQueryHandler : IRequestHandler<GetCoursesPageQuery, MyCoursesViewModel>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly IRepository<ExamCourse> _examCourseRepository;
        private readonly ILogger<GetCoursesPageQueryHandler> _logger;

        public GetCoursesPageQueryHandler(IRepository<Course> courseRepository, IRepository<ExamCourse> examCourseRepository, ILogger<GetCoursesPageQueryHandler> logger)
        {
            _courseRepository = courseRepository;
            _examCourseRepository = examCourseRepository;
            _logger = logger;
        }

        public async Task<MyCoursesViewModel> Handle(GetCoursesPageQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = _courseRepository.Entity().Include(c => c.CourseSections)
                    .ThenInclude(cs => cs.CourseLectures)
                .Where(c => c.IsPublished);
                if (request.ExamId.HasValue)
                {
                    var examCourseIds = await _examCourseRepository.Entity()
                        .Where(ec => ec.ExamId == request.ExamId.Value)
                        .Select(ec => ec.CourseId)
                        .ToListAsync(cancellationToken);
                    query = query.Where(c => examCourseIds.Contains(c.CourseId));
                }
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(c => c.CourseName.Contains(request.SearchTerm));
                }
               
                var rawCourses = await query.Select(c => new
                {
                    CourseId = c.CourseId,
                    Title = c.CourseName,
                    ThumbnailUrl = "/images/course-default.jpg", 
                    Description = c.Description ,
                    Price=c.Price,
                    IsFree=c.IsFree,
                    DiscountPrice=c.DiscountPrice,
                    TotalSections = c.CourseSections.Count(cs => cs.IsActive),
                    TotalLectures = c.CourseSections
                     .Where(cs => cs.IsActive)
                     .SelectMany(cs => cs.CourseLectures)
                     .Count(cl => cl.IsPublished)
                }).ToListAsync(cancellationToken);
                string? examName = null;
                var coursesList = rawCourses.Select(c =>
                {
                    int total = c.TotalLectures;   
                    return new EnrolledCourseDto
                    {
                        CourseId = c.CourseId,
                        Title = c.Title,
                        IsFree = c.IsFree,
                        DiscountPrice = c.DiscountPrice,
                        ThumbnailUrl = c.ThumbnailUrl,
                        TotalModules = c.TotalSections,
                        TotalLessons = total ,
                        Price = c.Price
                    };
                }).ToList();
                return new MyCoursesViewModel
                {
                    Courses = coursesList,  
                    SearchQuery = request.SearchTerm ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while fetching courses.");

                throw;
            }
        }
    }
}
