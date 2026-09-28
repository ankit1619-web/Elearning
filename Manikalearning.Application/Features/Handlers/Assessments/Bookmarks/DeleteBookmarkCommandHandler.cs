using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Bookmarks
{
    public class DeleteBookmarkCommandHandler : IRequestHandler<DeleteBookmarkCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Bookmark> _bookmarkRepo;

        public DeleteBookmarkCommandHandler(IRepository<Bookmark> bookmarkRepo)
        {
            _bookmarkRepo = bookmarkRepo;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteBookmarkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bookmark = await _bookmarkRepo.Entity()
                    .FirstOrDefaultAsync(b => b.BookmarkId == request.BookmarkId, cancellationToken);

                if (bookmark == null)
                    return ResponseDto<bool>.Failure("Bookmark record not found.");

                 _bookmarkRepo.Remove(bookmark);
                await _bookmarkRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Bookmark deleted.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Failed to remove bookmark: {ex.Message}");
            }
        }
    }
}
