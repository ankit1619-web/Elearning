using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class GetPerformanceQueryHandler : IRequestHandler<GetPerformanceQuery, PerformanceDto>
    {
        private readonly IRepository<TestAttempt> _attempts;
        public GetPerformanceQueryHandler(IRepository<TestAttempt> attempts) => _attempts = attempts;

        public async Task<PerformanceDto> Handle(GetPerformanceQuery req, CancellationToken ct)
        {
            var today = DateTime.UtcNow.Date;
            var rangeStart = today.AddDays(-(req.Days - 1));
            var loadFrom = today.AddDays(-(Math.Max(req.Days, 7) - 1));

            var all = await _attempts.Entity().AsNoTracking()
                .Where(a => a.UserId == req.UserId && a.Status == "Submitted" && a.SubmittedAt != null && a.SubmittedAt >= loadFrom)
                .OrderBy(a => a.SubmittedAt)
                .Select(a => new
                {
                    a.AttemptId,
                    SubmittedAt = a.SubmittedAt!.Value,
                    a.TotalQuestions,
                    a.AttemptedQuestions,
                    a.CorrectAnswers,
                    a.WrongAnswers,
                    a.SkippedQuestions,
                    a.Score,
                    a.TotalMarks,
                    a.Accuracy,
                    a.TimeTakenSeconds
                })
                .ToListAsync(ct);

            var inRange = all.Where(a => a.SubmittedAt >= rangeStart).ToList();

            var attempted = inRange.Sum(a => a.AttemptedQuestions);
            var correct = inRange.Sum(a => a.CorrectAnswers);
            var marks = inRange.Sum(a => a.TotalMarks); 

            var dates = await _attempts.Entity().AsNoTracking()
                .Where(a => a.UserId == req.UserId && a.Status == "Submitted" && a.SubmittedAt != null && a.SubmittedAt >= today.AddDays(-60))
                .Select(a => a.SubmittedAt!.Value.Date).Distinct().ToListAsync(ct);
            var set = dates.ToHashSet();
            var day = set.Contains(today) ? today : today.AddDays(-1);
            var streak = 0;
            while (set.Contains(day)) { streak++; day = day.AddDays(-1); }

            return new PerformanceDto
            {
                Days = req.Days,
                Attempts = inRange.Count,
                TotalQuestions = inRange.Sum(a => a.TotalQuestions),
                Attempted = attempted,
                Correct = correct,
                Wrong = inRange.Sum(a => a.WrongAnswers),
                Skipped = inRange.Sum(a => a.SkippedQuestions),
                Accuracy = attempted > 0 ? Math.Round(correct * 100m / attempted, 1) : 0,
                AvgScorePercent = marks > 0 ? Math.Round(inRange.Sum(a => a.Score) * 100m / marks, 1) : 0,
                TimeSpentSeconds = inRange.Sum(a => a.TimeTakenSeconds),
                StreakDays = streak,
                Recent = inRange.OrderByDescending(a => a.SubmittedAt).Take(8).Select(a => new AttemptPointDto
                {
                    AttemptId = a.AttemptId,
                    SubmittedAt = a.SubmittedAt,
                    TotalQuestions = a.TotalQuestions,
                    Score = a.Score,
                    TotalMarks = a.TotalMarks,
                    Accuracy = a.Accuracy
                }).ToList(),
                Daily = Enumerable.Range(0, 7).Select(i => today.AddDays(i - 6)).Select(d => new DayPointDto
                {
                    Date = d,
                    Questions = all.Where(a => a.SubmittedAt.Date == d).Sum(a => a.AttemptedQuestions)
                }).ToList()
            };
        }
    }
}
