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
    public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Plan> _planRepo;

        public DeletePlanCommandHandler(IRepository<Plan> planRepo)
        {
            _planRepo = planRepo;
        }

        public async Task<ResponseDto<bool>> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var plan = await _planRepo.Entity()
                    .FirstOrDefaultAsync(p => p.PlanId == request.PlanId, cancellationToken);

                if (plan == null)
                {
                    return ResponseDto<bool>.Failure("Subscription plan not found.");
                }

                  _planRepo.Remove(plan);
                await _planRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Subscription plan deleted successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to delete plan: {ex.Message}");
            }
        }
    }
}
