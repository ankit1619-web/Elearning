using MediatR;

namespace Manikalearning.Application.Features.Commands.Students
{
    public record RemoveMistakeCommand(Guid UserId, Guid MistakeId) : IRequest<bool>;
}
