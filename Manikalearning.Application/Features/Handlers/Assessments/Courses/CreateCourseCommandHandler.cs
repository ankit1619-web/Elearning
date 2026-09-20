using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Courses
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<CreateCourseCommandHandler> _logger;

        public CreateCourseCommandHandler(IRepository<Course> courseRepository, ILogger<CreateCourseCommandHandler> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var course = new Course
                {
                    CourseId = Guid.NewGuid(),
                    CourseName = request.CourseName,
                    Description = request.Description,
                    Price = request.Price,
                    CreatedAt = DateTime.UtcNow,
                    IsPublished = true
                };

                await _courseRepository.AddAsync(course);
                await _courseRepository.SaveChangesAsync();
                _logger.LogInformation("Course created successfully: {CourseId}", course.CourseId);

                return ResponseDto<Guid>.Success(course.CourseId, "Course created successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course: {CourseName}", request.CourseName);
                return ResponseDto<Guid>.Failure("An unexpected error occurred while creating the course.", new List<string> { ex.Message });
            }
        }
    }
}
