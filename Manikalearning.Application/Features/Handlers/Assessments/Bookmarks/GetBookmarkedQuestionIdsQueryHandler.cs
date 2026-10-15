using FluentValidation;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Bookmarks
{
    public record GetBookmarkedQuestionIdsQuery(Guid UserId, List<Guid> QuestionIds) : IRequest<HashSet<Guid>>;

    public class GetBookmarkedQuestionIdsQueryValidator : AbstractValidator<GetBookmarkedQuestionIdsQuery>
    {
        public GetBookmarkedQuestionIdsQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.QuestionIds).NotNull().Must(l => l.Count <= 500).WithMessage("Too many questions.");
        }
    }

    public class GetBookmarkedQuestionIdsQueryHandler : IRequestHandler<GetBookmarkedQuestionIdsQuery, HashSet<Guid>>
    {
        private readonly IRepository<Bookmark> _bookmarks;
        public GetBookmarkedQuestionIdsQueryHandler(IRepository<Bookmark> bookmarks) => _bookmarks = bookmarks;

        public async Task<HashSet<Guid>> Handle(GetBookmarkedQuestionIdsQuery req, CancellationToken ct)
        {
            var ids = await _bookmarks.Entity().AsNoTracking()
                .Where(b => b.UserId == req.UserId && b.QuestionId != null && req.QuestionIds.Contains(b.QuestionId.Value))
                .Select(b => b.QuestionId!.Value).ToListAsync(ct);
            return ids.ToHashSet();
        }
    }
}
