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
    public class GetCourseSectionsByCourseIdQueryHandler : IRequestHandler<GetCourseSectionsByCourseIdQuery, ResponseDto<List<CourseSectionDto>>>
    {
        private readonly IRepository<CourseSection> _sectionRepository;
        private readonly ILogger<GetCourseSectionsByCourseIdQueryHandler> _logger;

        public GetCourseSectionsByCourseIdQueryHandler(
            IRepository<CourseSection> sectionRepository,
            ILogger<GetCourseSectionsByCourseIdQueryHandler> logger)
        {
            _sectionRepository = sectionRepository;
            _logger = logger;
        }

        public async Task<ResponseDto<List<CourseSectionDto>>> Handle(GetCourseSectionsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var sections = await _sectionRepository.Entity()
                    .AsNoTracking()
                    .Where(s => s.CourseId == request.CourseId)
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new CourseSectionDto
                    {
                        CourseSectionId = s.CourseSectionId,
                        CourseId = s.CourseId,
                        SectionName = s.SectionName,
                        Description = s.Description,
                        DisplayOrder = s.DisplayOrder,
                        IsActive = s.IsActive
                    })
                    .ToListAsync(cancellationToken);

                return ResponseDto<List<CourseSectionDto>>.Success(sections, "Course sections fetched successfully.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("GetCourseSections operation was canceled for CourseId: {CourseId}", request.CourseId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching sections for CourseId: {CourseId}", request.CourseId);
                return ResponseDto<List<CourseSectionDto>>.Failure("An error occurred while fetching course sections.");
            }
        }
    }
}
