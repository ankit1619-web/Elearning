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
                var existing = await _bookmarkRepo.Entity()
                    .FirstOrDefaultAsync(b => b.UserId == request.UserId && b.QuestionId == request.QuestionId, cancellationToken);

                if (existing != null)
                {
                     _bookmarkRepo.Remove(existing);
                    await _bookmarkRepo.SaveChangesAsync();
                    return ResponseDto<bool>.Success(false, "Question removed from bookmarks.");
                }

                var bookmark = new Bookmark
                {
                    BookmarkId = Guid.NewGuid(),
                    UserId = request.UserId,
                    QuestionId = request.QuestionId,
                    CreatedAt = DateTime.UtcNow
                };

                await _bookmarkRepo.AddAsync(bookmark);
                await _bookmarkRepo.SaveChangesAsync();

                return ResponseDto<bool>.Success(true, "Question bookmarked successfully.");
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.Failure($"Error toggling bookmark: {ex.Message}");
            }
        }
    }
}
