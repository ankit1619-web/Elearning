using FluentValidation;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record RemoveBookmarkCommand(Guid UserId, Guid BookmarkId) : IRequest<bool>;

    public class RemoveBookmarkCommandValidator : AbstractValidator<RemoveBookmarkCommand>
    {
        public RemoveBookmarkCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.BookmarkId).NotEmpty();
        }
    }

    public class RemoveBookmarkCommandHandler : IRequestHandler<RemoveBookmarkCommand, bool>
    {
        private readonly IRepository<Bookmark> _bookmarks;
        public RemoveBookmarkCommandHandler(IRepository<Bookmark> bookmarks) => _bookmarks = bookmarks;

        public async Task<bool> Handle(RemoveBookmarkCommand req, CancellationToken ct)
        { 
            var item = await _bookmarks.Entity().FirstOrDefaultAsync(b => b.BookmarkId == req.BookmarkId && b.UserId == req.UserId, ct);
            if (item == null) return false;

            _bookmarks.Remove(item);                           // <- your IRepository delete method (Remove / Delete)
            await _bookmarks.SaveChangesAsync();
            return true;
        }
    }
}
