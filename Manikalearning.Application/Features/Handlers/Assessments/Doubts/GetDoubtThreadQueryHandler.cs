using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Doubts;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Doubts
{
    public class GetDoubtThreadQueryHandler : IRequestHandler<GetDoubtThreadQuery, DoubtThreadDto?>
    {
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<DoubtReplyEntity> _replies;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<User> _users;

        public GetDoubtThreadQueryHandler(IRepository<DoubtEntity> doubts, IRepository<DoubtReplyEntity> replies, IRepository<Course> courses, IRepository<User> users)
        { _doubts = doubts; _replies = replies; _courses = courses; _users = users; }

        public async Task<DoubtThreadDto?> Handle(GetDoubtThreadQuery req, CancellationToken ct)
        {
            var q = _doubts.Entity().AsNoTracking().Where(d => d.DoubtId == req.DoubtId && d.IsActive && !d.IsDeleted);
            if (req.UserId.HasValue) q = q.Where(d => d.UserId == req.UserId.Value);

            var row = await (from d in q
                             join c in _courses.Entity().AsNoTracking() on d.CourseId equals c.CourseId into cj
                             from c in cj.DefaultIfEmpty()
                             join u in _users.Entity().AsNoTracking() on d.UserId equals u.Id into uj
                             from u in uj.DefaultIfEmpty()
                             select new { d.DoubtId, d.QuestionText, d.Status, d.CreatedAt, CourseName = c != null ? c.CourseName : null, Student = u != null ? u.UserName : null })
                            .FirstOrDefaultAsync(ct);
            if (row == null) return null;

            var replies = await _replies.Entity().AsNoTracking()
                .Where(r => r.DoubtId == req.DoubtId && !r.IsDeleted)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new ReplyDto { ReplyId = r.ReplyId, Text = r.ReplyText, IsFromStaff = r.IsFromStaff, CreatedAt = r.CreatedAt })
                .ToListAsync(ct);

            return new DoubtThreadDto
            {
                Doubt = new DoubtDto
                {
                    DoubtId = row.DoubtId,
                    QuestionText = row.QuestionText,
                    Status = row.Status,
                    CreatedAt = row.CreatedAt,
                    CourseName = row.CourseName,
                    ReplyCount = replies.Count,
                    StudentName = req.UserId.HasValue ? null : row.Student        // students never need their own name
                },
                Replies = replies
            };
        }
    }
}
