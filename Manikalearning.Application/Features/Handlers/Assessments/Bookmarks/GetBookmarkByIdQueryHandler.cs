using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Bookmarks
{
    public class GetBookmarkByIdQueryHandler : IRequestHandler<GetBookmarkByIdQuery, ResponseDto<BookmarkDto>>
    {
        private readonly IRepository<Bookmark> _bookmarkRepo;

        public GetBookmarkByIdQueryHandler(IRepository<Bookmark> bookmarkRepo)
        {
            _bookmarkRepo = bookmarkRepo;
        }

        public async Task<ResponseDto<BookmarkDto>> Handle(GetBookmarkByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var bookmark = await _bookmarkRepo.Entity()
                    .Include(b => b.User)
                    .Include(b => b.Question)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.BookmarkId == request.BookmarkId, cancellationToken);

                if (bookmark == null)
                {
                    return ResponseDto<BookmarkDto>.Failure("Bookmark record not found.");
                }

                var dto = new BookmarkDto
                {
                    BookmarkId = bookmark.BookmarkId,
                    UserId = bookmark.UserId,
                    StudentName = bookmark.User?.FullName ?? "N/A",
                    StudentEmail = bookmark.User?.Email ?? "N/A",
                    QuestionId = bookmark.QuestionId,
                    QuestionText = bookmark.Question?.QuestionText ?? "N/A",
                    Explanation = bookmark.Question?.Explanation ?? "N/A",
                    SavedAt = bookmark.CreatedAt,
                    CorrectAnswer = bookmark.Question?.Options.FirstOrDefault(x => x.IsCorrect)?.OptionText ?? "N/A",
                };

                return ResponseDto<BookmarkDto>.Success(dto, "Bookmark details retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<BookmarkDto>.Failure($"Failed to fetch bookmark details: {ex.Message}");
            }
        }
    }
}
