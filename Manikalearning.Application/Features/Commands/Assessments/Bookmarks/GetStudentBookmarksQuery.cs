using Manikalearning.Application.DTOs;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Bookmarks
{
    public record GetStudentBookmarksQuery(
        string? Search = null,
        Guid? UserId = null,
        int PageIndex = 1,
        int PageSize = 10
    ) : IRequest<ResponseDto<BookmarkPagedResultDto>>;
}
