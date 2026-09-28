using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription
{
    public record ExtendSubscriptionCommand(
         Guid SubscriptionId,
         int AdditionalDays,
         string? Reason
     ) : IRequest<ResponseDto<bool>>;
}
