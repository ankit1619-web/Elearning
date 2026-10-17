using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Plans;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Plans
{
    public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Plan> _planRepo;
        private readonly IRepository<PlanFeature> _featureRepo; 
        public UpdatePlanCommandHandler(IRepository<Plan> planRepo, IRepository<PlanFeature> featureRepo)
        {
            _planRepo = planRepo;
            _featureRepo = featureRepo;
        } 

        public async Task<ResponseDto<bool>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingPlan = await _planRepo.Entity()
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);

                if (existingPlan == null)
                {
                    return ResponseDto<bool>.Failure("Subscription plan not found.");
                }

                existingPlan.PlanName = request.PlanName;
                existingPlan.Price = request.Price;
                existingPlan.DurationDays = request.DurationDays;
                existingPlan.Description = request.Description ?? "";
                existingPlan.IsActive = request.IsActive;


                var wanted = PlanFeatureHelper.Clean(request.Features);
                var existing = await _featureRepo.Entity()
                    .Where(f => f.PlanId == request.PlanId).ToListAsync(cancellationToken);

                var toRemove = existing.Where(e => !wanted.Contains(e.FeatureCode, StringComparer.OrdinalIgnoreCase)).ToList();
                var toAdd = wanted.Where(w => !existing.Any(e => e.FeatureCode.Equals(w, StringComparison.OrdinalIgnoreCase)))
                                  .Select(w => new PlanFeature { PlanId = request.PlanId, FeatureCode = w }).ToList();

                foreach (var f in toRemove) _featureRepo.Remove(f);          
                foreach (var f in toAdd) await _featureRepo.AddAsync(f);

                _planRepo.Update(existingPlan);
                await _planRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Subscription plan updated successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to update plan: {ex.Message}");
            }
        }
    }
}
