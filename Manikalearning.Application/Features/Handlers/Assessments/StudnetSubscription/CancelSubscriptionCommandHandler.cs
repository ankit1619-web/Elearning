using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.StudnetSubscription
{
    public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand, ResponseDto<bool>>
    {
        private readonly IRepository<StudentSubscription> _subscriptionRepo;

        public CancelSubscriptionCommandHandler(IRepository<StudentSubscription> subscriptionRepo)
        {
            _subscriptionRepo = subscriptionRepo;
        }

        public async Task<ResponseDto<bool>> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var sub = await _subscriptionRepo.Entity()
                    .FirstOrDefaultAsync(s => s.SubscriptionId == request.SubscriptionId, cancellationToken);

                if (sub == null)
                    return ResponseDto<bool>.Failure("Subscription record not found.");
                 

                  _subscriptionRepo.Update(sub);
                await _subscriptionRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Subscription cancelled successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to cancel subscription: {ex.Message}");
            }
        }
    }
}
