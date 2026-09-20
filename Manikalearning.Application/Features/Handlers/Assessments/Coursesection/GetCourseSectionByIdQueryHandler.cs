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
    public class GetCourseSectionByIdQueryHandler : IRequestHandler<GetCourseSectionByIdQuery, ResponseDto<CourseSectionDto>>
    {
        private readonly IRepository<CourseSection> _sectionRepository;
        private readonly ILogger<GetCourseSectionByIdQueryHandler> _logger;

        public GetCourseSectionByIdQueryHandler(
            IRepository<CourseSection> sectionRepository,
            ILogger<GetCourseSectionByIdQueryHandler> logger)
        {
            _sectionRepository = sectionRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<CourseSectionDto>> Handle(GetCourseSectionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var section = await _sectionRepository.Entity()
                    .AsNoTracking()
                    .Where(s => s.CourseSectionId == request.SectionId)
                    .Select(s => new CourseSectionDto
                    {
                        CourseSectionId = s.CourseSectionId,
                        CourseId = s.CourseId,
                        SectionName = s.SectionName,
                        Description = s.Description,
                        DisplayOrder = s.DisplayOrder,
                        IsActive = s.IsActive 
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (section == null)
                {
                    return ResponseDto<CourseSectionDto>.Failure("Course section not found.");
                }

                return ResponseDto<CourseSectionDto>.Success(section, "Course section fetched successfully.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetCourseSectionById operation was canceled for SectionId: {SectionId}", request.SectionId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching section with SectionId: {SectionId}", request.SectionId);
                return ResponseDto<CourseSectionDto>.Failure("An error occurred while fetching the course section.");
            }
        }
    }
}
