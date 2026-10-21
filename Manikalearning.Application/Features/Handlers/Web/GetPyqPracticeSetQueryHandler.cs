using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Manikalearning.Application.Features.Handlers.Web
{ 
    internal static class PyqHelper
    {
        public static string Slug(string name) =>
            Regex.Replace(name.Trim().ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');

        public static string Pick(bool hindi, string? en, string? hi) =>
            hindi && !string.IsNullOrWhiteSpace(hi) ? hi! : (en ?? "");

        // Public PYQ ke liye sirf valid questions
        public static IQueryable<Question> Pyq(IQueryable<Question> q) =>
            q.Where(x => x.IsPYQ && x.IsActive && x.IsApproved && x.Options.Count >= 2);
    }
     
    public record PyqExamCardVm(Guid Id, string Name, string Slug, int QuestionCount, int YearCount);
    public record PyqBankVm(string? Search, int TotalQuestions, List<PyqExamCardVm> Exams);

    public record PyqSubjectVm(Guid Id, string Name, int QuestionCount);
    public record PyqYearVm(int Year, int QuestionCount);
    public record PyqExamVm(Guid Id, string Name, string Slug, string? Description,
                            int TotalQuestions, List<PyqYearVm> Years, List<PyqSubjectVm> Subjects);

    public record PyqOptionVm(string Key, string Text);
    public record PyqQuestionVm(Guid Id, int? Year, string? Session, string Text, List<PyqOptionVm> Options);
    public record PyqPracticeVm(string Title, Guid? ExamId, Guid? SubjectId, int? Year, int Count, string Lang, List<PyqQuestionVm> Questions);

    public class AnswerInput
    {
        public Guid QuestionId { get; set; }
        public string? Selected { get; set; }  
    }

    public record PyqResultItemVm(Guid QuestionId, string? Selected, string Correct, bool IsCorrect, string? Explanation);
    public record PyqResultVm(int Total, int Attempted, int Correct, int Wrong, double Percentage, List<PyqResultItemVm> Items);
     
    public record GetPyqBankQuery(string? Search = null) : IRequest<PyqBankVm>;

    public class GetPyqBankQueryHandler : IRequestHandler<GetPyqBankQuery, PyqBankVm>
    {
        private readonly IRepository<Question> _questions;
        private readonly IRepository<Exam> _exams;

        public GetPyqBankQueryHandler(IRepository<Question> questions, IRepository<Exam> exams)
        {
            _questions = questions;
            _exams = exams;
        }

        public async Task<PyqBankVm> Handle(GetPyqBankQuery r, CancellationToken ct)
        {
            var rows = await PyqHelper.Pyq(_questions.Entity().AsNoTracking())
                .Where(q => q.ExamId != null)
                .GroupBy(q => new { q.ExamId, q.PYQYear })
                .Select(g => new { g.Key.ExamId, g.Key.PYQYear, Count = g.Count() })
                .ToListAsync(ct);

            var examIds = rows.Select(x => x.ExamId!.Value).Distinct().ToList();

            var exams = await _exams.Entity().AsNoTracking()
                .Where(e => examIds.Contains(e.ExamId))
                .Select(e => new { e.ExamId, e.ExamName })        
                .ToListAsync(ct);

            var cards = exams.Select(e =>
            {
                var mine = rows.Where(x => x.ExamId == e.ExamId).ToList();
                return new PyqExamCardVm(
                    e.ExamId, e.ExamName, PyqHelper.Slug(e.ExamName),
                    mine.Sum(x => x.Count),
                    mine.Where(x => x.PYQYear != null).Select(x => x.PYQYear).Distinct().Count());
            });

            var term = r.Search?.Trim();
            if (!string.IsNullOrEmpty(term))
                cards = cards.Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase));

            var list = cards.OrderByDescending(c => c.QuestionCount).ToList();
            return new PyqBankVm(term, list.Sum(c => c.QuestionCount), list);
        }
    }
     
    public record GetPyqExamQuery(string Slug) : IRequest<PyqExamVm?>;

    public class GetPyqExamQueryHandler : IRequestHandler<GetPyqExamQuery, PyqExamVm?>
    {
        private readonly IRepository<Question> _questions;
        private readonly IRepository<Exam> _exams;

        public GetPyqExamQueryHandler(IRepository<Question> questions, IRepository<Exam> exams)
        {
            _questions = questions;
            _exams = exams;
        }

        public async Task<PyqExamVm?> Handle(GetPyqExamQuery r, CancellationToken ct)
        { 
            var exams = await _exams.Entity().AsNoTracking()
                .Select(e => new { e.ExamId, e.ExamName })
                .ToListAsync(ct);

            var exam = exams.FirstOrDefault(e => PyqHelper.Slug(e.ExamName) == r.Slug.ToLowerInvariant());
            if (exam is null) return null;

            var qs = PyqHelper.Pyq(_questions.Entity().AsNoTracking()).Where(q => q.ExamId == exam.ExamId);

            var total = await qs.CountAsync(ct);
            if (total == 0) return null;

            var years = await qs.Where(q => q.PYQYear != null)
                .GroupBy(q => q.PYQYear!.Value)
                .Select(g => new PyqYearVm(g.Key, g.Count()))
                .OrderByDescending(y => y.Year)
                .ToListAsync(ct);

            var subjectRows = await qs.Where(q => q.SubjectId != null)
                .GroupBy(q => new { q.SubjectId, q.Subject!.SubjectName })
                .Select(g => new { g.Key.SubjectId, g.Key.SubjectName, Count = g.Count() })
                .ToListAsync(ct);

            var subjects = subjectRows
                .Select(s => new PyqSubjectVm(s.SubjectId!.Value, s.SubjectName ?? "", s.Count))
                .OrderBy(s => s.Name)
                .ToList();

            return new PyqExamVm(exam.ExamId, exam.ExamName, r.Slug, null, total, years, subjects);
        }
    }
     
    public record GetPyqPracticeSetQuery(Guid? ExamId, Guid? SubjectId, int? Year, int Count, string Lang = "en")
        : IRequest<PyqPracticeVm?>;

    public class GetPyqPracticeSetQueryHandler : IRequestHandler<GetPyqPracticeSetQuery, PyqPracticeVm?>
    {
        private readonly IRepository<Question> _questions;
        private readonly IRepository<Exam> _exams;
        private readonly IRepository<Subject> _subjects;

        public GetPyqPracticeSetQueryHandler(
            IRepository<Question> questions, IRepository<Exam> exams, IRepository<Subject> subjects)
        {
            _questions = questions;
            _exams = exams;
            _subjects = subjects;
        }

        public async Task<PyqPracticeVm?> Handle(GetPyqPracticeSetQuery r, CancellationToken ct)
        {
            var hindi = r.Lang == "hi";

            var query = PyqHelper.Pyq(_questions.Entity().AsNoTracking());
            if (r.ExamId.HasValue) query = query.Where(q => q.ExamId == r.ExamId);
            if (r.SubjectId.HasValue) query = query.Where(q => q.SubjectId == r.SubjectId);
            if (r.Year.HasValue) query = query.Where(q => q.PYQYear == r.Year);
             
            var rows = await query
                .OrderBy(q => Guid.NewGuid())
                .Take(r.Count)
                .Select(q => new
                {
                    q.QuestionId,
                    q.PYQYear,
                    q.PYQSession,
                    q.QuestionText,
                    q.QuestionTextHindi,
                    Options = q.Options.OrderBy(o => o.DisplayOrder)
                        .Select(o => new { o.OptionLabel, o.OptionText, o.OptionTextHindi })
                        .ToList()
                })
                .ToListAsync(ct);

            if (rows.Count == 0) return null;

            var examName = r.ExamId.HasValue
                ? await _exams.Entity().AsNoTracking()
                    .Where(e => e.ExamId == r.ExamId).Select(e => e.ExamName).FirstOrDefaultAsync(ct)
                : null;

            var subjectName = r.SubjectId.HasValue
                ? await _subjects.Entity().AsNoTracking()
                    .Where(s => s.SubjectId == r.SubjectId).Select(s => s.SubjectName).FirstOrDefaultAsync(ct)
                : null;

            var title = string.Join(" • ", new[]
            {
                examName ?? "All exams",
                subjectName ?? "All subjects",
                r.Year?.ToString() ?? "All years"
            });

            var questions = rows.Select(x => new PyqQuestionVm(
                x.QuestionId, x.PYQYear, x.PYQSession,
                PyqHelper.Pick(hindi, x.QuestionText, x.QuestionTextHindi),
                x.Options.Select(o => new PyqOptionVm(
                    o.OptionLabel, PyqHelper.Pick(hindi, o.OptionText, o.OptionTextHindi))).ToList()
            )).ToList();

            return new PyqPracticeVm(title, r.ExamId, r.SubjectId, r.Year, questions.Count, r.Lang, questions);
        }
    } 
    public record EvaluatePyqAnswersQuery(List<AnswerInput> Answers, string Lang = "en") : IRequest<PyqResultVm>;

    public class EvaluatePyqAnswersQueryHandler : IRequestHandler<EvaluatePyqAnswersQuery, PyqResultVm>
    {
        private readonly IRepository<Question> _questions;
        public EvaluatePyqAnswersQueryHandler(IRepository<Question> questions) => _questions = questions;

        public async Task<PyqResultVm> Handle(EvaluatePyqAnswersQuery r, CancellationToken ct)
        {
            var hindi = r.Lang == "hi";
            var ids = r.Answers.Select(a => a.QuestionId).Distinct().ToList();

            var keys = await _questions.Entity().AsNoTracking()
                .Where(q => q.IsPYQ && ids.Contains(q.QuestionId))
                .Select(q => new
                {
                    q.QuestionId,
                    Correct = q.Options.Where(o => o.IsCorrect)
                                       .OrderBy(o => o.DisplayOrder)
                                       .Select(o => o.OptionLabel)
                                       .FirstOrDefault(),
                    q.Explanation,
                    q.ExplanationHindi,
                    q.ShortExplanation,
                    q.ShortExplanationHindi
                })
                .ToDictionaryAsync(q => q.QuestionId, ct);

            var items = new List<PyqResultItemVm>();

            foreach (var a in r.Answers.DistinctBy(x => x.QuestionId))
            {
                if (!keys.TryGetValue(a.QuestionId, out var k) || k.Correct is null) continue;

                var selected = string.IsNullOrWhiteSpace(a.Selected) ? null : a.Selected.Trim();
                var isCorrect = selected != null &&
                                string.Equals(selected, k.Correct, StringComparison.OrdinalIgnoreCase);

                var full = PyqHelper.Pick(hindi, k.Explanation, k.ExplanationHindi);
                var expl = !string.IsNullOrWhiteSpace(full)
                    ? full
                    : PyqHelper.Pick(hindi, k.ShortExplanation, k.ShortExplanationHindi);

                items.Add(new PyqResultItemVm(
                    a.QuestionId, selected, k.Correct, isCorrect,
                    string.IsNullOrWhiteSpace(expl) ? null : expl));
            }

            var attempted = items.Count(i => i.Selected != null);
            var correct = items.Count(i => i.IsCorrect);
            var pct = items.Count == 0 ? 0 : Math.Round(correct * 100.0 / items.Count, 1);

            return new PyqResultVm(items.Count, attempted, correct, attempted - correct, pct, items);
        }
    }
}