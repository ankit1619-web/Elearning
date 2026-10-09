using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Bookmarks
{
    public class ToggleBookmarkCommandHandler : IRequestHandler<ToggleBookmarkCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Bookmark> _bookmarkRepo;

        public ToggleBookmarkCommandHandler(IRepository<Bookmark> bookmarkRepo)
        {
            _bookmarkRepo = bookmarkRepo;
        }

        public async Task<ResponseDto<bool>> Handle(ToggleBookmarkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingBookmark = await _bookmarkRepo.Query()
                .FirstOrDefaultAsync(b => b.UserId == request.UserId &&
                    ((request.LectureId.HasValue && b.LectureId == request.LectureId) ||
                     (request.QuestionId.HasValue && b.QuestionId == request.QuestionId)), cancellationToken);

                if (existingBookmark != null)
                {
                     _bookmarkRepo.Remove(existingBookmark);
                    await _bookmarkRepo.SaveChangesAsync();
                    return ResponseDto<bool>.Success(false, "Question removed from bookmarks.");
                }
                var itemType = request.LectureId.HasValue ? "Lecture" : "Question";
                var bookmark = new Bookmark
                {
                    BookmarkId = Guid.NewGuid(),
                    UserId = request.UserId,
                    LectureId = request.LectureId,
                    QuestionId = request.QuestionId,
                    ItemType = itemType,
                    CreatedAt = DateTime.UtcNow
                };
                await _bookmarkRepo.AddAsync(bookmark);
                await _bookmarkRepo.SaveChangesAsync();
                return ResponseDto<bool>.Success(true, $"{itemType} bookmarked successfully!"); 
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Error toggling bookmark: {ex.Message}");
            }
        }
    }
}
