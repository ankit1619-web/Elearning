using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription
{
    public record CancelSubscriptionCommand(Guid SubscriptionId) : IRequest<ResponseDto<bool>>;
}
