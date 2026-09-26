using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Plans
{
    public class GetPlanCourseMappingQueryHandler : IRequestHandler<GetPlanCourseMappingQuery, ResponseDto<PlanCourseMappingDto>>
    {
        private readonly IRepository<Plan> _planRepo;
        private readonly IRepository<Course> _courseRepo;
        private readonly IRepository<PlanCourse> _planCourseRepo;

        public GetPlanCourseMappingQueryHandler(
            IRepository<Plan> planRepo,
            IRepository<Course> courseRepo,
            IRepository<PlanCourse> planCourseRepo)
        {
            _planRepo = planRepo;
            _courseRepo = courseRepo;
            _planCourseRepo = planCourseRepo;
        }

        public async Task<ResponseDto<PlanCourseMappingDto>> Handle(GetPlanCourseMappingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = await _planRepo.Entity()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);

                if (plan == null)
                    return ResponseDto<PlanCourseMappingDto>.Failure("Subscription plan not found.");

                var mappedCourseIds = await _planCourseRepo.Entity()
                    .Where(pc => pc.PlanId == request.PlanId)
                    .Select(pc => pc.CourseId)
                    .ToListAsync(cancellationToken);

                var allCourses = await _courseRepo.Entity()
                    .AsNoTracking()
                    .OrderBy(c => c.CourseName)
                    .Select(c => new CourseOptionDto
                    {
                        CourseId = c.CourseId,
                        CourseName = c.CourseName,
                        Price = c.Price,
                        IsMapped = mappedCourseIds.Contains(c.CourseId)
                    }).ToListAsync(cancellationToken);

                var dto = new PlanCourseMappingDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    SelectedCourseIds = mappedCourseIds,
                    AvailableCourses = allCourses
                };

                return ResponseDto<PlanCourseMappingDto>.Success(dto, "Plan course mappings fetched successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<PlanCourseMappingDto>.Failure($"Failed to retrieve mappings: {ex.Message}");
            }
        }
    }
}
