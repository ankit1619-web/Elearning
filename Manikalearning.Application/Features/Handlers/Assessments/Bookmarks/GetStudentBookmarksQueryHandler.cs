using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Bookmarks;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Bookmarks
{
    public class GetStudentBookmarksQueryHandler : IRequestHandler<GetStudentBookmarksQuery, ResponseDto<BookmarkPagedResultDto>>
    {
        private readonly IRepository<Bookmark> _bookmarkRepo;

        public GetStudentBookmarksQueryHandler(IRepository<Bookmark> bookmarkRepo)
        {
            _bookmarkRepo = bookmarkRepo;
        }

        public async Task<ResponseDto<BookmarkPagedResultDto>> Handle(GetStudentBookmarksQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _bookmarkRepo.Entity()
                    .Include(b => b.User)
                    .Include(b => b.Question)
                    .AsNoTracking();

                if (request.UserId.HasValue && request.UserId != Guid.Empty)
                {
                    query = query.Where(b => b.UserId == request.UserId.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var term = request.Search.Trim().ToLower();
                    query = query.Where(b =>
                        b.User.FullName.ToLower().Contains(term) ||
                        b.User.Email.ToLower().Contains(term) ||
                        b.Question.QuestionText.ToLower().Contains(term));
                }

                var totalCount = await query.CountAsync(cancellationToken);

                var items = await query
                    .OrderByDescending(b => b.CreatedAt)
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(b => new BookmarkDto
                    {
                        BookmarkId = b.BookmarkId,
                        UserId = b.UserId,
                        StudentName = b.User.FullName ?? "N/A",
                        StudentEmail = b.User.Email ?? "N/A",
                        QuestionId = b.QuestionId,
                        LectureId = b.LectureId,
                        QuestionText = b.Question!.QuestionText ?? "N/A",
                        SavedAt = b.CreatedAt,
                        Explanation = b.Question!.Explanation ?? "N/A",
                        CorrectAnswer = b.Question!.Options.FirstOrDefault(x => x.IsCorrect)!.OptionText ?? "N/A",
                        
                    }).ToListAsync(cancellationToken);

                var result = new BookmarkPagedResultDto
                {
                    Bookmarks = items,
                    TotalCount = totalCount,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Search = request.Search,
                };

                return ResponseDto<BookmarkPagedResultDto>.Success(result, "Bookmarks fetched successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<BookmarkPagedResultDto>.Failure($"Failed to fetch bookmarks: {ex.Message}");
            }
        }
    }
}
