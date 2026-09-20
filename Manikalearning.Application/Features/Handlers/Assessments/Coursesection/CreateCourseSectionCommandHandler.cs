using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Coursesection;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Coursesection
{
    public class CreateCourseSectionCommandHandler : IRequestHandler<CreateCourseSectionCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<CourseSection> _sectionRepository;
        private readonly ILogger<CreateCourseSectionCommandHandler> _logger;

        public CreateCourseSectionCommandHandler(
            IRepository<CourseSection> sectionRepository,
            ILogger<CreateCourseSectionCommandHandler> logger)
        {
            _sectionRepository = sectionRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateCourseSectionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var newSection = new CourseSection
                {
                    CourseSectionId = Guid.NewGuid(),
                    CourseId = request.CourseId,
                    SectionName = request.Title.Trim(),
                    Description = request.Description ?? "",
                    DisplayOrder = request.DisplayOrder,
                    IsActive = true 
                };

                await _sectionRepository.AddAsync(newSection);
                await _sectionRepository.SaveChangesAsync();

                _logger.LogInformation("New CourseSection '{Title}' created with ID: {SectionId}", newSection.SectionName, newSection.CourseSectionId);

                return ResponseDto<Guid>.Success(newSection.CourseSectionId, "Course section created successfully.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("CreateCourseSection operation was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating section for CourseId: {CourseId}", request.CourseId);
                return ResponseDto<Guid>.Failure("An error occurred while creating the course section.");
            }
        }
    }
}
