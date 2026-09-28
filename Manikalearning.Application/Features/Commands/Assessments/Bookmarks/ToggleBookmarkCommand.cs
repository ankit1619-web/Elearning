using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Bookmarks
{
    public record ToggleBookmarkCommand(Guid UserId, Guid QuestionId) : IRequest<ResponseDto<bool>>;
}
