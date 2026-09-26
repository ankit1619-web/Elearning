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
    public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Plan> _planRepo;

        public CreatePlanCommandHandler(IRepository<Plan> planRepo)
        {
            _planRepo = planRepo;
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
