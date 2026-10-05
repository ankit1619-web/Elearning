using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record GetMockTestDetailQuery(Guid TestId) : IRequest<MockTestDetailDto?>;
}
