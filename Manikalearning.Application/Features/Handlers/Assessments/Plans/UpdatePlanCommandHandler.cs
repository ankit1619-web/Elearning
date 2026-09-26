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
    public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Plan> _planRepo;

        public UpdatePlanCommandHandler(IRepository<Plan> planRepo)
        {
            _planRepo = planRepo;
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
                existingPlan.Description = request.Description;
                existingPlan.IsActive = request.IsActive;

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
