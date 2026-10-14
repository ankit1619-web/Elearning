using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public record GetMyDoubtsQuery(Guid UserId, string? Status = null, int Page = 1, int PageSize = 10) : IRequest<DoubtListDto>;

    public class GetMyDoubtsQueryValidator : AbstractValidator<GetMyDoubtsQuery>
    {
        public GetMyDoubtsQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 50);
            RuleFor(x => x.Status).MaximumLength(50);
        }
    }

    public class GetMyDoubtsQueryHandler : IRequestHandler<GetMyDoubtsQuery, DoubtListDto>
    {
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<DoubtReplyEntity> _replies;
        public GetMyDoubtsQueryHandler(IRepository<DoubtEntity> doubts, IRepository<Course> courses, IRepository<DoubtReplyEntity> replies)
        { _doubts = doubts; _courses = courses; _replies = replies; }

        public async Task<DoubtListDto> Handle(GetMyDoubtsQuery req, CancellationToken ct)
        {
            var status = string.IsNullOrWhiteSpace(req.Status) ? null : req.Status.Trim();

            var q = _doubts.Entity().AsNoTracking().Where(d => d.UserId == req.UserId && d.IsActive && !d.IsDeleted);
            if (status != null) q = q.Where(d => d.Status == status);

            var total = await q.CountAsync(ct);
            var page = await (from d in q
                              join c in _courses.Entity().AsNoTracking() on d.CourseId equals c.CourseId into cj
                              from c in cj.DefaultIfEmpty()                       // course is optional
                              orderby d.CreatedAt descending
                              select new DoubtDto
                              {
                                  DoubtId = d.DoubtId,
                                  QuestionText = d.QuestionText,
                                  Status = d.Status,
                                  CourseName = c != null ? c.CourseName : null,
                                  CreatedAt = d.CreatedAt
                              })
                              .Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync(ct);

            var ids = page.Select(x => x.DoubtId).ToList();
            var counts = await _replies.Entity().AsNoTracking().Where(r => ids.Contains(r.DoubtId) && !r.IsDeleted)
                .GroupBy(r => r.DoubtId).Select(g => new { Id = g.Key, N = g.Count() }).ToListAsync(ct);
            var byId = counts.ToDictionary(x => x.Id, x => x.N);
            foreach (var d in page) d.ReplyCount = byId.TryGetValue(d.DoubtId, out var n) ? n : 0;

            return new DoubtListDto { Items = page, Status = status, Page = req.Page, PageSize = req.PageSize, TotalCount = total };
        }
    }
}
