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

namespace Manikalearning.Application.Features.Handlers.Students
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
            var existing = await _bookmarkRepo.Query()
                .FirstOrDefaultAsync(b => b.UserId == request.UserId , cancellationToken);

            if (existing != null)
            {
                  _bookmarkRepo.Remove(existing);
                return ResponseDto<bool>.Success(false, "Bookmark removed successfully.");
            }

            await _bookmarkRepo.AddAsync(new Bookmark
            {
                BookmarkId = Guid.NewGuid(),
                UserId = request.UserId, 
                QuestionId = request.QuestionId,
                CreatedAt = DateTime.UtcNow
            });

            return ResponseDto<bool>.Success(true, "Bookmark added successfully.");
        }
    }
}
