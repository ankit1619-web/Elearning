using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Courses;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Courses
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeletePlanCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Course> _courseRepository;
        private readonly ILogger<DeleteCourseCommandHandler> _logger;

        public DeleteCourseCommandHandler(
            IRepository<Course> courseRepository,
            ILogger<DeleteCourseCommandHandler> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            try
            { 
                var course = await _courseRepository.Entity()
                    .FirstOrDefaultAsync(c => c.CourseId == request.CourseId, cancellationToken);
                 
                if (course == null)
                {
                    return ResponseDto<bool>.Failure("Course not found or has already been deleted.");
                }
                  
                  _courseRepository.Remove(course);
                 
                   await _courseRepository.SaveChangesAsync();
                 

                _logger.LogInformation("Course with ID: {CourseId} deleted successfully.", request.CourseId);

                return ResponseDto<bool>.Success(true, "Course deleted successfully.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DeleteCourse operation was canceled for CourseId: {CourseId}", request.CourseId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the course with ID: {CourseId}", request.CourseId);
                return ResponseDto<bool>.Failure("An error occurred while deleting the course. Please try again later.");
            }
        }
    }
}
