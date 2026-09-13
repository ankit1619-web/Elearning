using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Web;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Web
{ 
    public class GetCoursesPageQueryHandler : IRequestHandler<GetCoursesPageQuery, List<CourseDto>>
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

        public async Task<List<CourseDto>> Handle(GetCoursesPageQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<Course> query; 
                string? examName = null;
                if (request.ExamId.HasValue)
                {
                    var examCourse = await _examCourseRepository
                      .Query()
                      .AsNoTracking()
                      .Include(x => x.Exam)
                      .Where(x => x.ExamId == request.ExamId.Value)
                      .Select(x => new
                      {
                          x.CourseId,
                          ExamName = x.Exam != null
                              ? x.Exam.ExamName
                              : null
                      })
                      .ToListAsync(cancellationToken);

                    examName = examCourse
                        .Select(x => x.ExamName)
                        .FirstOrDefault();

                    var courseIds = examCourse
                        .Select(x => x.CourseId)
                        .ToList();  
                    query = _courseRepository
                        .Query()
                        .Where(x => courseIds.Contains(x.CourseId));
                }
                else
                {
                    query = _courseRepository.Query();
                }

                var courses = await query
                    .AsNoTracking()
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync(cancellationToken);

                return courses
                    .Select(x => new CourseDto
                    {
                        Id = x.CourseId,
                        ExamName=examName,
                        CourseName = x.CourseName,
                        Description = x.Description,
                        ShortDescription = x.Description,  
                        Price = x.Price 
                    })
                    .ToList();
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
