using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    internal class Teacherdashboard
    {
    }
    public class DashboardCourseRow
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public int Students { get; set; }
    }

    public class DashboardAttemptRow
    {
        public string StudentName { get; set; } = "";
        public string TestName { get; set; } = "";
        public DateTime StartedAt { get; set; }
    }

    public class DashboardDayPoint
    {
        public DateTime Day { get; set; }
        public int Attempts { get; set; }
    }

    public class TeacherDashboardDto
    {
        public string TeacherName { get; set; } = "";
        public int Courses { get; set; }
        public int Students { get; set; }
        public int Questions { get; set; }
        public int Tests { get; set; }
        public int PublishedTests { get; set; }
        public int OpenDoubts { get; set; }
        public int AttemptsLast14Days { get; set; }
        public List<DashboardDayPoint> Daily { get; set; } = new();
        public List<DashboardCourseRow> TopCourses { get; set; } = new();
        public List<DashboardAttemptRow> RecentAttempts { get; set; } = new();
    }

    public record GetTeacherDashboardQuery(Guid TeacherId) : IRequest<TeacherDashboardDto>;

    public class GetTeacherDashboardQueryHandler : IRequestHandler<GetTeacherDashboardQuery, TeacherDashboardDto>
    {
        private readonly IRepository<TUser> _users;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TStudentCourse> _sc;
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestAttempt> _attempts;
        private readonly IRepository<TDoubt> _doubts;

        public GetTeacherDashboardQueryHandler(IRepository<TUser> u, IRepository<TCourse> c, IRepository<TStudentCourse> sc,
            IRepository<TQuestion> q, IRepository<TTest> t, IRepository<TTestAttempt> a, IRepository<TDoubt> d)
        { _users = u; _courses = c; _sc = sc; _questions = q; _tests = t; _attempts = a; _doubts = d; }

        public async Task<TeacherDashboardDto> Handle(GetTeacherDashboardQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId;
            var dto = new TeacherDashboardDto();

            dto.TeacherName = await _users.Entity().AsNoTracking().AnyAsync(u => u.Id == tid, ct) ? await _users.Entity().AsNoTracking()
                .Where(u => u.Id == tid).Select(u => u.FullName).FirstOrDefaultAsync(ct) ?? "Teacher" : "Teacher";

            var courses = await _courses.Entity().AsNoTracking().Where(c => c.CreatedBy == tid)
                .Select(c => new { c.CourseId, c.CourseName }).ToListAsync(ct);
            var courseIds = courses.Select(c => c.CourseId).ToList();
            dto.Courses = courses.Count;
             
            var enrol = await _sc.Entity().AsNoTracking()
                .Where(x => courseIds.Contains(x.CourseId) && x.IsActive)
                .Select(x => new { x.CourseId, x.UserId }).Distinct().ToListAsync(ct);
            dto.Students = enrol.Select(x => x.UserId).Distinct().Count();
            dto.TopCourses = courses.Select(c => new DashboardCourseRow
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                Students = enrol.Count(e => e.CourseId == c.CourseId)
            }).OrderByDescending(x => x.Students).ThenBy(x => x.CourseName).Take(5).ToList();

            dto.Questions = await _questions.Entity().AsNoTracking().CountAsync(q => q.CreatedBy == tid, ct);

            var tests = await _tests.Entity().AsNoTracking().Where(t => t.CreatedBy == tid)
                .Select(t => new { t.TestId, t.TestName, t.IsPublished }).ToListAsync(ct);
            dto.Tests = tests.Count;
            dto.PublishedTests = tests.Count(t => t.IsPublished);

            dto.OpenDoubts = await _doubts.Entity().AsNoTracking()
                .CountAsync(d => courseIds.Contains((Guid)d.CourseId!) && d.Status == "Open", ct);

            if (tests.Count == 0) return dto;
            var testIds = tests.Select(t => t.TestId).ToList();
             
            var today = DateTime.Today;
            var from = today.AddDays(-13);
            var recent = await _attempts.Entity().AsNoTracking()
                .Where(a => testIds.Contains((Guid)a.TestId!) && a.StartedAt >= from)
                .Select(a => a.StartedAt).ToListAsync(ct);
            dto.AttemptsLast14Days = recent.Count;
            dto.Daily = Enumerable.Range(0, 14).Select(i => from.AddDays(i))
                .Select(d => new DashboardDayPoint { Day = d, Attempts = recent.Count(x => x.Date == d) }).ToList();
             
            var latest = await _attempts.Entity().AsNoTracking().Where(a => testIds.Contains((Guid)a.TestId!))
                .OrderByDescending(a => a.StartedAt).Take(6)
                .Select(a => new { a.UserId, a.TestId, a.StartedAt }).ToListAsync(ct);
            var uids = latest.Select(x => x.UserId).Distinct().ToList();
            var names = await _users.Entity().AsNoTracking().Where(u => uids.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName }).ToListAsync(ct);
            var nameMap = names.ToDictionary(x => x.Id, x => x.FullName);
            var testMap = tests.ToDictionary(x => x.TestId, x => x.TestName);
            dto.RecentAttempts = latest.Select(x => new DashboardAttemptRow
            {
                StudentName = nameMap.GetValueOrDefault(x.UserId) ?? "Student",
                TestName = testMap.GetValueOrDefault((Guid)x.TestId!) ?? "Test",
                StartedAt = x.StartedAt
            }).ToList();

            return dto;
        }
    }
}
