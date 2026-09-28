using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Bookmarks
{
    public record GetBookmarkByIdQuery(Guid BookmarkId) : IRequest<ResponseDto<BookmarkDto>>;
}
