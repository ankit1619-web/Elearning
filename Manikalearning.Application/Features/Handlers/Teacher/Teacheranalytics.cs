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
    internal class Teacheranalytics
    {
    }
    public class TestPerformanceRow
    {
        public Guid TestId { get; set; }
        public string TestName { get; set; } = "";
        public int Attempts { get; set; }
        public decimal? AvgPercent { get; set; }
        public decimal? HighPercent { get; set; }
        public decimal? LowPercent { get; set; }
    }

    public class TopicAccuracyRow
    {
        public string TopicName { get; set; } = "";
        public int Answered { get; set; }
        public int Correct { get; set; }
        public decimal AccuracyPercent { get; set; }
    }

    public class HardQuestionRow
    {
        public string QuestionText { get; set; } = "";
        public string? TopicName { get; set; }
        public int Answered { get; set; }
        public decimal AccuracyPercent { get; set; }
    }

    public class TeacherAnalyticsDto
    {
        public int Students { get; set; }
        public int PublishedTests { get; set; }
        public int Attempts { get; set; }
        public decimal? AvgPercent { get; set; }
        public Guid? SelectedTestId { get; set; }
        public List<LookupItem> TestOptions { get; set; } = new();
        public List<TestPerformanceRow> Tests { get; set; } = new();
        public List<TopicAccuracyRow> Topics { get; set; } = new();
        public List<HardQuestionRow> HardQuestions { get; set; } = new();
    }

    public record GetTeacherAnalyticsQuery(Guid TeacherId, Guid? TestId = null) : IRequest<TeacherAnalyticsDto>;

    public class GetTeacherAnalyticsQueryHandler : IRequestHandler<GetTeacherAnalyticsQuery, TeacherAnalyticsDto>
    {
        private const int MinAnswersForHardQuestion = 5;

        private readonly IRepository<TStudentCourse> _studentCourses;
        private readonly IRepository<TCourse> _courses;
        private readonly IRepository<TTest> _tests;
        private readonly IRepository<TTestAttempt> _attempts;
        private readonly IRepository<TTestAttemptAnswer> _answers;
        private readonly IRepository<TQuestion> _questions;
        private readonly IRepository<TTopic> _topics;

        public GetTeacherAnalyticsQueryHandler(IRepository<TStudentCourse> sc, IRepository<TCourse> c, IRepository<TTest> t,
            IRepository<TTestAttempt> a, IRepository<TTestAttemptAnswer> ans, IRepository<TQuestion> q, IRepository<TTopic> tp)
        { _studentCourses = sc; _courses = c; _tests = t; _attempts = a; _answers = ans; _questions = q; _topics = tp; }

        public async Task<TeacherAnalyticsDto> Handle(GetTeacherAnalyticsQuery r, CancellationToken ct)
        {
            var tid = r.TeacherId;
            var dto = new TeacherAnalyticsDto();
             
            var tests = await _tests.Entity().AsNoTracking().Where(t => t.CreatedBy == tid)
                .OrderBy(t => t.TestName)
                .Select(t => new { t.TestId, t.TestName, t.TotalMarks, t.IsPublished }).ToListAsync(ct);

            dto.TestOptions = tests.Select(t => new LookupItem(t.TestId, t.TestName)).ToList();
            dto.PublishedTests = tests.Count(t => t.IsPublished);
             
            var testId = r.TestId != null && tests.Any(t => t.TestId == r.TestId) ? r.TestId : null;
            dto.SelectedTestId = testId;
             
            dto.Students = await (from sc in _studentCourses.Entity().AsNoTracking()
                                  from c in _courses.Entity().AsNoTracking()
                                  where sc.CourseId == c.CourseId && c.CreatedBy == tid && sc.IsActive
                                  select sc.UserId).Distinct().CountAsync(ct);

            if (tests.Count == 0) return dto;
             
            var attempts = await (from a in _attempts.Entity().AsNoTracking()
                                  from t in _tests.Entity().AsNoTracking()
                                  where a.TestId == t.TestId && t.CreatedBy == tid
                                  select new { a.AttemptId, a.TestId, t.TotalMarks }).ToListAsync(ct);

            var marksRows = await (from ans in _answers.Entity().AsNoTracking()
                                   from a in _attempts.Entity().AsNoTracking()
                                   from t in _tests.Entity().AsNoTracking()
                                   where ans.AttemptId == a.AttemptId && a.TestId == t.TestId && t.CreatedBy == tid
                                   group ans by ans.AttemptId into g
                                   select new { AttemptId = g.Key, Marks = g.Sum(x => x.MarksObtained) }).ToListAsync(ct);
            var marks = marksRows.ToDictionary(x => x.AttemptId, x => x.Marks);

            decimal? Pct(decimal total, Guid attemptId)
                => total > 0 ? Math.Round(marks.GetValueOrDefault(attemptId) / total * 100, 1) : null;

            var scored = attempts.Select(a => new { a.TestId, Pct = Pct(a.TotalMarks, a.AttemptId) }).ToList();
             
            dto.Tests = tests.Select(t =>
            {
                var mine = scored.Where(x => x.TestId == t.TestId).ToList();
                var pcts = mine.Where(x => x.Pct.HasValue).Select(x => x.Pct!.Value).ToList();
                return new TestPerformanceRow
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    Attempts = mine.Count,
                    AvgPercent = pcts.Count > 0 ? Math.Round(pcts.Average(), 1) : null,
                    HighPercent = pcts.Count > 0 ? pcts.Max() : null,
                    LowPercent = pcts.Count > 0 ? pcts.Min() : null
                };
            }).OrderByDescending(x => x.Attempts).ThenBy(x => x.TestName).ToList();
             
            var filtered = scored.Where(x => testId == null || x.TestId == testId).ToList();
            dto.Attempts = filtered.Count;
            var fp = filtered.Where(x => x.Pct.HasValue).Select(x => x.Pct!.Value).ToList();
            dto.AvgPercent = fp.Count > 0 ? Math.Round(fp.Average(), 1) : null;

            if (dto.Attempts == 0) return dto;
             
            var topicAgg = await (from ans in _answers.Entity().AsNoTracking()
                                  from a in _attempts.Entity().AsNoTracking()
                                  from t in _tests.Entity().AsNoTracking()
                                  from q in _questions.Entity().AsNoTracking()
                                  where ans.AttemptId == a.AttemptId && a.TestId == t.TestId && ans.QuestionId == q.QuestionId
                                        && t.CreatedBy == tid && (testId == null || t.TestId == testId)
                                        && ans.IsAnswered && q.TopicId != null
                                  group ans by q.TopicId into g
                                  select new { TopicId = g.Key, Answered = g.Count(), Correct = g.Count(x => (bool)x.IsCorrect!) })
                .ToListAsync(ct);

            var topicIds = topicAgg.Select(x => x.TopicId!.Value).ToList();
            var topicNames = await _topics.Entity().AsNoTracking().Where(t => topicIds.Contains(t.TopicId))
                .Select(t => new { t.TopicId, t.TopicName }).ToListAsync(ct);
            var topicMap = topicNames.ToDictionary(x => x.TopicId, x => x.TopicName);

            dto.Topics = topicAgg.Where(x => x.Answered > 0).Select(x => new TopicAccuracyRow
            {
                TopicName = topicMap.GetValueOrDefault(x.TopicId!.Value) ?? "Unknown topic",
                Answered = x.Answered,
                Correct = x.Correct,
                AccuracyPercent = Math.Round((decimal)x.Correct / x.Answered * 100, 1)
            }).OrderBy(x => x.AccuracyPercent).ThenByDescending(x => x.Answered).Take(15).ToList();
             
            var hard = await (from ans in _answers.Entity().AsNoTracking()
                              from a in _attempts.Entity().AsNoTracking()
                              from t in _tests.Entity().AsNoTracking()
                              where ans.AttemptId == a.AttemptId && a.TestId == t.TestId
                                    && t.CreatedBy == tid && (testId == null || t.TestId == testId) && ans.IsAnswered
                              group ans by ans.QuestionId into g
                              select new { QuestionId = g.Key, Answered = g.Count(), Correct = g.Count(x => (bool)x.IsCorrect!) })
                .Where(x => x.Answered >= MinAnswersForHardQuestion)
                .OrderBy(x => (decimal)x.Correct / x.Answered)
                .Take(10).ToListAsync(ct);

            if (hard.Count > 0)
            {
                var qIds = hard.Select(x => x.QuestionId).ToList();
                var qInfo = await _questions.Entity().AsNoTracking().Where(q => qIds.Contains(q.QuestionId))
                    .Select(q => new { q.QuestionId, q.QuestionText, q.TopicId }).ToListAsync(ct);

                var qTopicIds = qInfo.Where(x => x.TopicId != null).Select(x => x.TopicId!.Value).Distinct().ToList();
                var qTopicNames = await _topics.Entity().AsNoTracking().Where(t => qTopicIds.Contains(t.TopicId))
                    .Select(t => new { t.TopicId, t.TopicName }).ToListAsync(ct);
                var qTopicMap = qTopicNames.ToDictionary(x => x.TopicId, x => x.TopicName);
                var qMap = qInfo.ToDictionary(x => x.QuestionId);

                dto.HardQuestions = hard.Where(h => qMap.ContainsKey(h.QuestionId)).Select(h => new HardQuestionRow
                {
                    QuestionText = qMap[h.QuestionId].QuestionText,
                    TopicName = qMap[h.QuestionId].TopicId != null ? qTopicMap.GetValueOrDefault(qMap[h.QuestionId].TopicId!.Value) : null,
                    Answered = h.Answered,
                    AccuracyPercent = Math.Round((decimal)h.Correct / h.Answered * 100, 1)
                }).ToList();
            }

            return dto;
        }
    }
}
