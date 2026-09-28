using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription
{
    public record GetStudentSubscriptionByIdQuery(Guid SubscriptionId) : IRequest<ResponseDto<StudentSubscriptionDto>>;
}
