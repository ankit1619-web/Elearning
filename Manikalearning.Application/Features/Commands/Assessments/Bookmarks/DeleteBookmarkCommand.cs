using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Bookmarks
{
    public record DeleteBookmarkCommand(Guid BookmarkId) : IRequest<ResponseDto<bool>>;
}
