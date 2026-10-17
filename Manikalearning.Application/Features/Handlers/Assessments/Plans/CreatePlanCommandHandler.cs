using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Plans
{
    public static class PlanFeatureHelper
    {
        public static List<string> Clean(IEnumerable<string>? codes) =>
            (codes ?? Enumerable.Empty<string>())
                .Select(c => (c ?? "").Trim())
                .Select(c => PlanFeatureCodes.All.FirstOrDefault(a => a.Equals(c, StringComparison.OrdinalIgnoreCase)))
                .Where(c => c != null).Select(c => c!)
                .Distinct().ToList();
    }
    public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Plan> _planRepo;
        private readonly IRepository<PlanFeature> _featureRepo;

        public CreatePlanCommandHandler(IRepository<Plan> planRepo, IRepository<PlanFeature> featureRepo)
        {
            _planRepo = planRepo;
            _featureRepo = featureRepo;
        } 

        public async Task<ResponseDto<Guid>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = new Plan
                {
                    PlanId = Guid.NewGuid(),
                    PlanName = request.PlanName,
                    Price = request.Price,
                    DurationDays = request.DurationDays,
                    Description = request.Description,
                    IsActive = request.IsActive
                }; 
                await _planRepo.AddAsync(plan);

                foreach (var code in PlanFeatureHelper.Clean(request.Features))
                    await _featureRepo.AddAsync(new PlanFeature { PlanId = plan.PlanId, FeatureCode = code });

                await _planRepo.SaveChangesAsync();

                return ResponseDto<Guid>.Success(plan.PlanId, "Subscription plan created successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<Guid>.Failure($"Failed to create plan: {ex.Message}");
            }
        }
    }
}
