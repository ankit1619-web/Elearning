using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Users
{
    public record ToggleUserStatusCommand(string UserId) : IRequest<bool>;
}
