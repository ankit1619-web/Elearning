using FluentValidation;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    public  class TeacherDoubts
    {
    }

    public record GetTeacherDoubtsQuery(Guid TeacherId, string? Status = null,
    Guid? CourseId = null, int Page = 1, int PageSize = 10) : IRequest<StaffDoubtListDto>;

    public class GetTeacherDoubtsQueryValidator : AbstractValidator<GetTeacherDoubtsQuery>
    {
        public GetTeacherDoubtsQueryValidator()
        {
            RuleFor(x => x.TeacherId).NotEmpty();
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(5, 50);
        }
    }

    public class GetTeacherDoubtsQueryHandler : IRequestHandler<GetTeacherDoubtsQuery, StaffDoubtListDto>
    {
        private readonly IRepository<TDoubt> _doubts;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TReply> _replies;
        private readonly IRepository<TUser> _users;

        public GetTeacherDoubtsQueryHandler(IRepository<TDoubt> d, IRepository<TCourse> c, IRepository<TReply> r, IRepository<TUser> u)
        { _doubts = d; _courses = c; _replies = r; _users = u; }

        public async Task<StaffDoubtListDto> Handle(GetTeacherDoubtsQuery req, CancellationToken ct)
        {
            var status = string.IsNullOrWhiteSpace(req.Status) ? null : req.Status.Trim();
            var courseId = req.CourseId;
            var tid = req.TeacherId;

            var q = from d in _doubts.Entity().AsNoTracking()
                    from c in _courses.Entity().AsNoTracking()
                    from u in _users.Entity().AsNoTracking()
                    where c.CourseId == d.CourseId && u.Id == d.UserId
                          && c.CreatedBy == tid && d.IsActive && !d.IsDeleted
                          && (status == null || d.Status == status)
                          && (courseId == null || c.CourseId == courseId)
                    select new { d, c, u };

            var total = await q.CountAsync(ct);
             
            var items = await q.OrderBy(x => x.d.Status == DoubtStatus.Open ? 0 : 1)
                .ThenBy(x => x.d.CreatedAt)
                .Skip((req.Page - 1) * req.PageSize).Take(req.PageSize)
                .Select(x => new DoubtDto
                {
                    DoubtId = x.d.DoubtId,
                    QuestionText = x.d.QuestionText,
                    Status = x.d.Status,
                    CourseName = x.c.CourseName,
                    StudentName = x.u.FullName,
                    CreatedAt = x.d.CreatedAt
                }).ToListAsync(ct);

            var ids = items.Select(i => i.DoubtId).ToList();
            var counts = await _replies.Entity().AsNoTracking()
                .Where(r => ids.Contains(r.DoubtId) && !r.IsDeleted)
                .GroupBy(r => r.DoubtId).Select(g => new { g.Key, N = g.Count() }).ToListAsync(ct);
            var map = counts.ToDictionary(x => x.Key, x => x.N);
            foreach (var i in items) i.ReplyCount = map.GetValueOrDefault(i.DoubtId);

            return new StaffDoubtListDto
            {
                Items = items,
                Status = status,
                Page = req.Page,
                PageSize = req.PageSize,
                TotalCount = total
            };
        }
    }
     
    public record GetTeacherDoubtThreadQuery(Guid TeacherId, Guid DoubtId) : IRequest<DoubtThreadDto?>;

    public class GetTeacherDoubtThreadQueryHandler : IRequestHandler<GetTeacherDoubtThreadQuery, DoubtThreadDto?>
    {
        private readonly IRepository<TDoubt> _doubts;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TReply> _replies;
        private readonly IRepository<TUser> _users;

        public GetTeacherDoubtThreadQueryHandler(IRepository<TDoubt> d, IRepository<TCourse> c, IRepository<TReply> r, IRepository<TUser> u)
        { _doubts = d; _courses = c; _replies = r; _users = u; }

        public async Task<DoubtThreadDto?> Handle(GetTeacherDoubtThreadQuery req, CancellationToken ct)
        {
            var tid = req.TeacherId;
            var doubtId = req.DoubtId;

            var doubt = await (from d in _doubts.Entity().AsNoTracking()
                               from c in _courses.Entity().AsNoTracking()
                               from u in _users.Entity().AsNoTracking()
                               where d.DoubtId == doubtId && c.CourseId == d.CourseId && u.Id == d.UserId
                                     && c.CreatedBy == tid && d.IsActive && !d.IsDeleted
                               select new DoubtDto
                               {
                                   DoubtId = d.DoubtId,
                                   QuestionText = d.QuestionText,
                                   Status = d.Status,
                                   CourseName = c.CourseName,
                                   StudentName = u.FullName,
                                   CreatedAt = d.CreatedAt
                               }).FirstOrDefaultAsync(ct);

            if (doubt == null) return null;

            var replies = await _replies.Entity().AsNoTracking()
                .Where(r => r.DoubtId == doubtId && !r.IsDeleted)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new ReplyDto
                {
                    ReplyId = r.ReplyId,
                    Text = r.ReplyText,
                    IsFromStaff = r.IsFromStaff,
                    CreatedAt = r.CreatedAt
                }).ToListAsync(ct);

            doubt.ReplyCount = replies.Count;
            return new DoubtThreadDto { Doubt = doubt, Replies = replies };
        }
    }
     
    public record AnswerDoubtCommand(Guid DoubtId, Guid StaffUserId, string ReplyText,
        bool IsAdmin = false) : IRequest<TeacherActionResult>;

    public class AnswerDoubtCommandValidator : AbstractValidator<AnswerDoubtCommand>
    {
        public AnswerDoubtCommandValidator()
        {
            RuleFor(x => x.DoubtId).NotEmpty();
            RuleFor(x => x.StaffUserId).NotEmpty();
            RuleFor(x => x.ReplyText).NotEmpty().MaximumLength(2000);  
        }
    }

    public class AnswerDoubtCommandHandler : IRequestHandler<AnswerDoubtCommand, TeacherActionResult>
    {
        private readonly IRepository<TDoubt> _doubts;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TReply> _replies;

        public AnswerDoubtCommandHandler(IRepository<TDoubt> d, IRepository<TCourse> c, IRepository<TReply> r)
        { _doubts = d; _courses = c; _replies = r; }

        public async Task<TeacherActionResult> Handle(AnswerDoubtCommand req, CancellationToken ct)
        {
            var doubt = await _doubts.Entity()
                .FirstOrDefaultAsync(d => d.DoubtId == req.DoubtId && d.IsActive && !d.IsDeleted, ct);
            if (doubt == null) return TeacherActionResult.Fail("Doubt not found.");
             
            if (!req.IsAdmin)
            {
                var staffId = req.StaffUserId;
                var doubtId = doubt.DoubtId;
                var owns = await (from d in _doubts.Entity().AsNoTracking()
                                  from c in _courses.Entity().AsNoTracking()
                                  where d.DoubtId == doubtId && c.CourseId == d.CourseId && c.CreatedBy == staffId
                                  select d.DoubtId).AnyAsync(ct);
                if (!owns) return TeacherActionResult.Fail("Doubt not found.");  
            }

            if (doubt.Status == DoubtStatus.Closed)
                return TeacherActionResult.Fail("This doubt is closed.");

            var reply = new TReply
            {
                ReplyId = Guid.NewGuid(),
                DoubtId = doubt.DoubtId,
                UserId = req.StaffUserId,
                ReplyText = req.ReplyText.Trim(),
                IsFromStaff = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            doubt.Status = DoubtStatus.Answered;  

            await _replies.AddRow(reply);
            await _doubts.UpdateRow(doubt);
            return TeacherActionResult.Ok("Reply sent.");
        }
    }
     
    public record CloseDoubtCommand(Guid DoubtId, Guid StaffUserId, bool IsAdmin = false) : IRequest<TeacherActionResult>;

    public class CloseDoubtCommandHandler : IRequestHandler<CloseDoubtCommand, TeacherActionResult>
    {
        private readonly IRepository<TDoubt> _doubts;
        private readonly IRepository<TCourse> _courses;
        public CloseDoubtCommandHandler(IRepository<TDoubt> d, IRepository<TCourse> c) { _doubts = d; _courses = c; }

        public async Task<TeacherActionResult> Handle(CloseDoubtCommand req, CancellationToken ct)
        {
            var doubt = await _doubts.Entity()
                .FirstOrDefaultAsync(d => d.DoubtId == req.DoubtId && d.IsActive && !d.IsDeleted, ct);
            if (doubt == null) return TeacherActionResult.Fail("Doubt not found.");

            if (!req.IsAdmin)
            {
                var staffId = req.StaffUserId;
                var doubtId = doubt.DoubtId;
                var owns = await (from d in _doubts.Entity().AsNoTracking()
                                  from c in _courses.Entity().AsNoTracking()
                                  where d.DoubtId == doubtId && c.CourseId == d.CourseId && c.CreatedBy == staffId
                                  select d.DoubtId).AnyAsync(ct);
                if (!owns) return TeacherActionResult.Fail("Doubt not found.");
            }

            doubt.Status = DoubtStatus.Closed;
            await _doubts.UpdateRow(doubt);
            return TeacherActionResult.Ok("Doubt closed.");
        }
    }
}
