using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.StudnetSubscription
{
    public record GetStudentSubscriptionsQuery(
        string? Search = null,
        string? StatusFilter = null,
        int PageIndex = 1,
        int PageSize = 10
    ) : IRequest<ResponseDto<StudentSubscriptionPagedResultDto>>;
}
