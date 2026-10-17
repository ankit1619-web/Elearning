using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using Microsoft.EntityFrameworkCore; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Plans
{
    public class GetPlanByIdQueryHandler : IRequestHandler<GetPlanByIdQuery, ResponseDto<PlanDto>>
    {
        private readonly IRepository<Plan> _planRepo;
        private readonly IRepository<PlanFeature> _featureRepo;

        public GetPlanByIdQueryHandler(IRepository<Plan> planRepo, IRepository<PlanFeature> featureRepo)
        {
            _planRepo = planRepo;
            _featureRepo = featureRepo;
        }

        public async Task<ResponseDto<PlanDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = await _planRepo.Entity()
                    .Include(p => p.PlanCourses)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);

                if (plan == null)
                    return ResponseDto<PlanDto>.Failure("Subscription plan not found.");
                var featureCodes = await _featureRepo.Entity().AsNoTracking()
                                   .Where(f => f.PlanId == request.PlanId)
                                   .Select(f => f.FeatureCode)
                                   .ToListAsync(cancellationToken);
                var dto = new PlanDto
                {
                    PlanId = plan.PlanId,
                    PlanName = plan.PlanName,
                    Price = plan.Price,
                    DurationDays = plan.DurationDays,
                    Description = plan.Description,
                    IsActive = plan.IsActive,
                    TotalCoursesMapped = plan.PlanCourses?.Count ?? 0,
                    Features = featureCodes
                };

                return ResponseDto<PlanDto>.Success(dto, "Plan details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<PlanDto>.Failure($"Error fetching plan details: {ex.Message}");
            }
        }
    }
}
