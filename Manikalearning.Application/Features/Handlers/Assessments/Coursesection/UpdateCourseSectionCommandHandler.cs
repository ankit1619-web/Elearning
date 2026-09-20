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
    public class UpdateCourseSectionCommandHandler : IRequestHandler<UpdateCourseSectionCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<CourseSection> _sectionRepository;
        private readonly ILogger<UpdateCourseSectionCommandHandler> _logger;

        public UpdateCourseSectionCommandHandler(
            IRepository<CourseSection> sectionRepository,
            ILogger<UpdateCourseSectionCommandHandler> logger)
        {
            _sectionRepository = sectionRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(UpdateCourseSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _sectionRepository.Entity()
                    .FirstOrDefaultAsync(s => s.CourseSectionId == request.SectionId, cancellationToken);

                if (section == null)
                {
                    return ResponseDto<Guid>.Failure("Course section not found.");
                }

                section.SectionName = request.Title.Trim();
                section.Description = request.Description ?? "";
                section.DisplayOrder = request.DisplayOrder;
                section.IsActive = request.IsActive;

                 _sectionRepository.Update(section);
                await _sectionRepository.SaveChangesAsync();

                _logger.LogInformation("CourseSection with ID: {SectionId} updated successfully.", request.SectionId);

                return ResponseDto<Guid>.Success(section.CourseSectionId, "Course section updated successfully.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("UpdateCourseSection operation was canceled for SectionId: {SectionId}", request.SectionId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating section with ID: {SectionId}", request.SectionId);
                return ResponseDto<Guid>.Failure("An error occurred while updating the course section.");
            }
        }
    }
}
