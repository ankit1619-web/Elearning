using Manikalearning.Application.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Teacher
{
    internal class Teacherstudents
    {
    }
    public class TeacherStudentListItem
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public List<string> Courses { get; set; } = new();
        public int TestsAttempted { get; set; }
        public decimal? AvgPercent { get; set; }
    }

    public class StudentAttemptRow
    {
        public Guid AttemptId { get; set; }
        public string TestName { get; set; } = "";
        public DateTime? StartedAt { get; set; }
        public decimal MarksObtained { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Percent { get; set; }
        public int Correct { get; set; }
        public int Wrong { get; set; }
        public int Unattempted { get; set; }
    }

    public class TeacherStudentDetailDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = "";
        public string? Email { get; set; }
        public List<string> Courses { get; set; } = new();
        public List<StudentAttemptRow> Attempts { get; set; } = new();
        public decimal? AvgPercent => Attempts.Count == 0 ? null : Math.Round(Attempts.Average(a => a.Percent), 1);
    }
     
    //  LIST 
    public record GetTeacherStudentsQuery(Guid TeacherId, string? Search = null, Guid? CourseId = null,
        int Page = 1, int PageSize = 10) : IRequest<PagedResult<TeacherStudentListItem>>;

    public class GetTeacherStudentsQueryHandler : IRequestHandler<GetTeacherStudentsQuery, PagedResult<TeacherStudentListItem>>
    {
        private readonly IRepository<TStudentCourse> _studentCourses;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TUser> _users;
        private readonly IRepository<TTestAttempt> _attempts;
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestAttemptAnswer> _answers;

        public GetTeacherStudentsQueryHandler(IRepository<TStudentCourse> sc, IRepository<TCourse> c, IRepository<TUser> u,
            IRepository<TTestAttempt> a, IRepository<TTest> t, IRepository<TTestAttemptAnswer> ans)
        { _studentCourses = sc; _courses = c; _users = u; _attempts = a; _tests = t; _answers = ans; }

        public async Task<PagedResult<TeacherStudentListItem>> Handle(GetTeacherStudentsQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId;
            var courseId = r.CourseId;
            var search = r.Search?.Trim();
            var page = Math.Max(1, r.Page);
            var size = Math.Clamp(r.PageSize, 5, 50);

            var enroll = from sc in _studentCourses.Entity().AsNoTracking()
                         from c in _courses.Entity().AsNoTracking()
                         where sc.CourseId == c.CourseId && c.CreatedBy == tid && sc.IsActive
                               && (courseId == null || c.CourseId == courseId)
                         select new { sc.UserId, c.CourseId, c.CourseName };

            var userIds = enroll.Select(x => x.UserId).Distinct();
            var users = _users.Entity().AsNoTracking()
                .Where(u => userIds.Contains(u.Id)
                            && (string.IsNullOrEmpty(search) || u.FullName.Contains(search) || u.Email.Contains(search)));

            var total = await users.CountAsync(ct);
            var items = await users.OrderBy(u => u.FullName)
                .Skip((page - 1) * size).Take(size)
                .Select(u => new TeacherStudentListItem { UserId = u.Id, FullName = u.FullName, Email = u.Email })
                .ToListAsync(ct);

            var ids = items.Select(i => i.UserId).ToList();
            if (ids.Count > 0)
            {
                var courseRows = await enroll.Where(x => ids.Contains(x.UserId))
                    .Select(x => new { x.UserId, x.CourseName }).Distinct().ToListAsync(ct);
                var courseMap = courseRows.GroupBy(x => x.UserId)
                    .ToDictionary(g => g.Key, g => g.Select(z => z.CourseName).OrderBy(n => n).ToList());

                var attempts = await (from a in _attempts.Entity().AsNoTracking()
                                      from t in _tests.Entity().AsNoTracking()
                                      where a.TestId == t.TestId && t.CreatedBy == tid && ids.Contains(a.UserId)
                                      select new { a.AttemptId, a.UserId, t.TotalMarks }).ToListAsync(ct);

                var aIds = attempts.Select(x => x.AttemptId).ToList();
                var marks = aIds.Count == 0
                    ? new Dictionary<Guid, decimal>()
                    : (await _answers.Entity().AsNoTracking().Where(x => aIds.Contains(x.AttemptId))
                        .GroupBy(x => x.AttemptId).Select(g => new { g.Key, Marks = g.Sum(z => z.MarksObtained) })
                        .ToListAsync(ct)).ToDictionary(x => x.Key, x => x.Marks);

                foreach (var i in items)
                {
                    i.Courses = courseMap.GetValueOrDefault(i.UserId) ?? new();
                    var mine = attempts.Where(a => a.UserId == i.UserId).ToList();
                    i.TestsAttempted = mine.Count;
                    var scored = mine.Where(a => a.TotalMarks > 0).ToList();
                    if (scored.Count > 0)
                        i.AvgPercent = Math.Round(scored.Average(a => marks.GetValueOrDefault(a.AttemptId) / a.TotalMarks * 100), 1);
                }
            }

            return new PagedResult<TeacherStudentListItem> { Items = items, Page = page, PageSize = size, TotalCount = total };
        }
    }
     
    //  DETAIL (student + attempts on this teacher's tests) 
    public record GetTeacherStudentDetailQuery(Guid TeacherId, Guid UserId) : IRequest<TeacherStudentDetailDto?>;

    public class GetTeacherStudentDetailQueryHandler : IRequestHandler<GetTeacherStudentDetailQuery, TeacherStudentDetailDto?>
    {
        private readonly IRepository<TStudentCourse> _studentCourses;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TUser> _users;
        private readonly IRepository<TTestAttempt> _attempts;
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestAttemptAnswer> _answers;

        public GetTeacherStudentDetailQueryHandler(IRepository<TStudentCourse> sc, IRepository<TCourse> c, IRepository<TUser> u,
            IRepository<TTestAttempt> a, IRepository<TTest> t, IRepository<TTestAttemptAnswer> ans)
        { _studentCourses = sc; _courses = c; _users = u; _attempts = a; _tests = t; _answers = ans; }

        public async Task<TeacherStudentDetailDto?> Handle(GetTeacherStudentDetailQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId; var uid = r.UserId;
             
            var courseNames = await (from sc in _studentCourses.Entity().AsNoTracking()
                                     from c in _courses.Entity().AsNoTracking()
                                     where sc.CourseId == c.CourseId && c.CreatedBy == tid && sc.IsActive && sc.UserId == uid
                                     select c.CourseName).Distinct().ToListAsync(ct);
            if (courseNames.Count == 0) return null;

            var user = await _users.Entity().AsNoTracking().Where(u => u.Id == uid)
                .Select(u => new { u.Id, u.FullName, u.Email }).FirstOrDefaultAsync(ct);
            if (user == null) return null;

            var attempts = await (from a in _attempts.Entity().AsNoTracking()
                                  from t in _tests.Entity().AsNoTracking()
                                  where a.TestId == t.TestId && t.CreatedBy == tid && a.UserId == uid
                                  orderby a.StartedAt descending
                                  select new { a.AttemptId, t.TestName, t.TotalMarks, t.TotalQuestions, a.StartedAt })
                .ToListAsync(ct);

            var aIds = attempts.Select(x => x.AttemptId).ToList();
            var agg = aIds.Count == 0
                ? new Dictionary<Guid, (decimal Marks, int Correct, int Answered)>()
                : (await _answers.Entity().AsNoTracking().Where(x => aIds.Contains(x.AttemptId))
                    .GroupBy(x => x.AttemptId)
                    .Select(g => new
                    {
                        g.Key,
                        Marks = g.Sum(z => z.MarksObtained),
                        Correct = g.Count(z => (bool)z.IsCorrect!),
                        Answered = g.Count(z => z.IsAnswered)
                    }).ToListAsync(ct)).ToDictionary(x => x.Key, x => (x.Marks, x.Correct, x.Answered));

            var rows = attempts.Select(a =>
            {
                var v = agg.GetValueOrDefault(a.AttemptId);
                return new StudentAttemptRow
                {
                    AttemptId = a.AttemptId,
                    TestName = a.TestName,
                    StartedAt = a.StartedAt,
                    MarksObtained = v.Marks,
                    TotalMarks = a.TotalMarks,
                    Percent = a.TotalMarks > 0 ? Math.Round(v.Marks / a.TotalMarks * 100, 1) : 0,
                    Correct = v.Correct,
                    Wrong = Math.Max(0, v.Answered - v.Correct),
                    Unattempted = Math.Max(0, a.TotalQuestions - v.Answered)
                };
            }).ToList();

            return new TeacherStudentDetailDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Courses = courseNames.OrderBy(n => n).ToList(),
                Attempts = rows
            };
        }
    }
}
