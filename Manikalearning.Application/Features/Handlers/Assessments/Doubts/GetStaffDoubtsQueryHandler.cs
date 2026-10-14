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
    public class GetStaffDoubtsQueryHandler : IRequestHandler<GetStaffDoubtsQuery, StaffDoubtListDto>
    {
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<DoubtReplyEntity> _replies;
        private readonly IRepository<Course> _courses;
        private readonly IRepository<User> _users;

        public GetStaffDoubtsQueryHandler(IRepository<DoubtEntity> doubts, IRepository<DoubtReplyEntity> replies, IRepository<Course> courses, IRepository<User> users)
        { _doubts = doubts; _replies = replies; _courses = courses; _users = users; }

        public async Task<StaffDoubtListDto> Handle(GetStaffDoubtsQuery req, CancellationToken ct)
        {
            var status = string.IsNullOrWhiteSpace(req.Status) ? null : req.Status.Trim();
            var q = _doubts.Entity().AsNoTracking().Where(d => d.IsActive && !d.IsDeleted);
            if (status != null) q = q.Where(d => d.Status == status);

            var total = await q.CountAsync(ct); 
            var page = await (from d in q
                              join c in _courses.Entity().AsNoTracking() on d.CourseId equals c.CourseId into cj
                              from c in cj.DefaultIfEmpty()
                              join u in _users.Entity().AsNoTracking() on d.UserId equals u.Id into uj
                              from u in uj.DefaultIfEmpty()
                              orderby (d.Status == DoubtStatus.Open ? 0 : 1), d.CreatedAt
                              select new DoubtDto
                              {
                                  DoubtId = d.DoubtId,
                                  QuestionText = d.QuestionText,
                                  Status = d.Status,
                                  CreatedAt = d.CreatedAt,
                                  CourseName = c != null ? c.CourseName : null,
                                  StudentName = u != null ? u.UserName : null
                              })
                             .Skip((req.Page - 1) * req.PageSize).Take(req.PageSize).ToListAsync(ct);

            var ids = page.Select(x => x.DoubtId).ToList();
            var counts = await _replies.Entity().AsNoTracking().Where(r => ids.Contains(r.DoubtId) && !r.IsDeleted)
                .GroupBy(r => r.DoubtId).Select(g => new { Id = g.Key, N = g.Count() }).ToListAsync(ct);
            var byId = counts.ToDictionary(x => x.Id, x => x.N);
            foreach (var d in page) d.ReplyCount = byId.TryGetValue(d.DoubtId, out var n) ? n : 0;

            return new StaffDoubtListDto { Items = page, Status = status, Page = req.Page, PageSize = req.PageSize, TotalCount = total };
        }
    }
}
