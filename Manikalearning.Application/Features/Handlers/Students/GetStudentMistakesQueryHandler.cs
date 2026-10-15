using Manikalearning.Application.Comman;
using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetMyDashboardQueryHandler : IRequestHandler<GetMyDashboardQuery, MyDashboardDto>
    {
        private readonly IRepository<User> _users;
        private readonly IRepository<TestAttempt> _attempts;
        private readonly IRepository<QuestionMistake> _mistakes;
        private readonly IRepository<Bookmark> _bookmarks;
        private readonly IRepository<DoubtEntity> _doubts;
        private readonly IRepository<StudentProfile> _profiles;
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<StudentSubscription> _subs;
        private readonly IRepository<Plan> _plans;

        public GetMyDashboardQueryHandler(IRepository<User> users, IRepository<TestAttempt> attempts, IRepository<QuestionMistake> mistakes,
            IRepository<Bookmark> bookmarks, IRepository<Doubt> doubts, IRepository<StudentProfile> profiles, IRepository<Exam> exams,
            IRepository<StudentSubscription> subs, IRepository<Plan> plans)
        { _users = users; _attempts = attempts; _mistakes = mistakes; _bookmarks = bookmarks; _doubts = doubts; _profiles = profiles; _exams = exams; _subs = subs; _plans = plans; }

        public async Task<MyDashboardDto> Handle(GetMyDashboardQuery req, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            var today = AppClock.TodayIst;
            var d = new MyDashboardDto();

            d.FullName = await _users.Entity().AsNoTracking().Where(u => u.Id == req.UserId).Select(u => u.FullName).FirstOrDefaultAsync(ct) ?? "";
             
            var profile = await _profiles.Entity().AsNoTracking().FirstOrDefaultAsync(p => p.UserId == req.UserId, ct);
            if (profile != null)
            {
                d.DailyGoal = profile.DailyGoal;
                if (profile.TargetExamId != null)
                    d.TargetExamName = await _exams.Entity().AsNoTracking().Where(e => e.ExamId == profile.TargetExamId).Select(e => e.ExamName).FirstOrDefaultAsync(ct);
                if (profile.TargetExamDate != null)
                {
                    var left = (profile.TargetExamDate.Value.Date - today).Days;
                    if (left >= 0) d.DaysLeft = left;
                }
            }

            // ---- practice ----
            var finished = AttemptRules.Finished(_attempts.Entity().AsNoTracking().Where(a => a.UserId == req.UserId));

            var totalAttempted = await finished.SumAsync(a => (int?)a.AttemptedQuestions, ct) ?? 0;
            var totalCorrect = await finished.SumAsync(a => (int?)a.CorrectAnswers, ct) ?? 0;
            d.SolvedTotal = totalAttempted;
            d.Accuracy = totalAttempted > 0 ? Math.Round(totalCorrect * 100m / totalAttempted, 1) : 0;

            var since = AppClock.IstDateStartUtc(today.AddDays(-60));
            var recent = await finished.Where(a => a.SubmittedAt >= since)
                .Select(a => new { At = a.SubmittedAt!.Value, a.AttemptedQuestions }).ToListAsync(ct);

            d.StreakDays = StreakCalculator.Compute(recent.Select(r => r.At));
            d.Week = Enumerable.Range(0, 7).Select(i => today.AddDays(i - 6)).Select(day => new DayPointDto
            {
                Date = day,
                Questions = recent.Where(r => AppClock.ToIst(r.At).Date == day).Sum(r => r.AttemptedQuestions)
            }).ToList();
            d.SolvedToday = d.Week.Last().Questions;
            d.SolvedWeek = d.Week.Sum(w => w.Questions);

            // ---- revise / saved / doubts ----
            d.Mistakes = await _mistakes.Entity().AsNoTracking().CountAsync(m => m.UserId == req.UserId && !m.IsResolved, ct);
            d.Saved = await _bookmarks.Entity().AsNoTracking().CountAsync(b => b.UserId == req.UserId, ct);
            d.OpenDoubts = await _doubts.Entity().AsNoTracking().CountAsync(x => x.UserId == req.UserId && x.IsActive && !x.IsDeleted && x.Status == "Open", ct);
            d.AnsweredDoubts = await _doubts.Entity().AsNoTracking().CountAsync(x => x.UserId == req.UserId && x.IsActive && !x.IsDeleted && x.Status == "Answered", ct);

            // ---- plan ----
            var plan = await (from s in _subs.Entity().AsNoTracking()
                              join p in _plans.Entity().AsNoTracking() on s.PlanId equals p.PlanId
                              where s.UserId == req.UserId && s.Status == SubStatus.Active && s.StartDate <= now && s.EndDate >= now
                              orderby s.EndDate descending
                              select new { p.PlanName, s.EndDate }).FirstOrDefaultAsync(ct);
            if (plan != null)
            {
                d.PlanName = plan.PlanName;
                d.PlanEnds = plan.EndDate;
                d.PlanDaysLeft = (int)Math.Ceiling((plan.EndDate - now).TotalDays);
            }
            return d;
        }
    }
    public record GetHeaderStatsQuery(Guid UserId) : IRequest<HeaderStatsDto>;

    public class GetHeaderStatsQueryHandler : IRequestHandler<GetHeaderStatsQuery, HeaderStatsDto>
    {
        private readonly IRepository<User> _users;
        private readonly IRepository<TestAttempt> _attempts;
        private readonly ISubscriptionChecker _checker;

        public GetHeaderStatsQueryHandler(IRepository<User> users, IRepository<TestAttempt> attempts, ISubscriptionChecker checker)
        { _users = users; _attempts = attempts; _checker = checker; }

        public async Task<HeaderStatsDto> Handle(GetHeaderStatsQuery req, CancellationToken ct)
        {
            var u = await _users.Entity().AsNoTracking().Where(x => x.Id == req.UserId)
                .Select(x => new { x.FullName, x.ProfileImage }).FirstOrDefaultAsync(ct);

            var since = AppClock.IstDateStartUtc(AppClock.TodayIst.AddDays(-60));
            var times = await AttemptRules.Finished(_attempts.Entity().AsNoTracking().Where(a => a.UserId == req.UserId))
                .Where(a => a.SubmittedAt >= since).Select(a => a.SubmittedAt!.Value).ToListAsync(ct);

            return new HeaderStatsDto
            {
                FullName = u?.FullName ?? "",
                ProfileImage = u?.ProfileImage,
                StreakDays = StreakCalculator.Compute(times),
                Features = await _checker.GetFeaturesAsync(req.UserId, ct)
            };
        }
    }
}
