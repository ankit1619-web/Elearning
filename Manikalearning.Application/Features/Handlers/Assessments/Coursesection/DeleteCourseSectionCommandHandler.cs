using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Coursesection;
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

namespace Manikalearning.Application.Features.Handlers.Assessments.Coursesection
{
    public class DeleteCourseSectionCommandHandler : IRequestHandler<DeleteCourseSectionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<CourseSection> _sectionRepository;
        private readonly ILogger<DeleteCourseSectionCommandHandler> _logger;

        public DeleteCourseSectionCommandHandler(
            IRepository<CourseSection> sectionRepository,
            ILogger<DeleteCourseSectionCommandHandler> logger)
        {
            _sectionRepository = sectionRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteCourseSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _sectionRepository.Entity() 
                    .FirstOrDefaultAsync(s => s.CourseSectionId == request.SectionId, cancellationToken);

                if (section == null)
                {
                    return ResponseDto<bool>.Failure("Course section not found or already deleted.");
                }
                 

                  _sectionRepository.Remove(section);
                await _sectionRepository.SaveChangesAsync();

                _logger.LogInformation("CourseSection with ID: {SectionId} deleted successfully.", request.SectionId);

                return ResponseDto<bool>.Success(true, "Course section deleted successfully.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("DeleteCourseSection operation was canceled for SectionId: {SectionId}", request.SectionId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting section with ID: {SectionId}", request.SectionId);
                return ResponseDto<bool>.Failure("An error occurred while deleting the course section.");
            }
        }
    }
}
