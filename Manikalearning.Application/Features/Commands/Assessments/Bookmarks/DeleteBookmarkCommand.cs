using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Bookmarks
{
    public record DeleteBookmarkCommand(Guid BookmarkId, Guid? Userid = null) : IRequest<ResponseDto<bool>>
    { 
        public DeleteBookmarkCommand(Guid bookmarkId) : this(bookmarkId, null) { }
    }
}
