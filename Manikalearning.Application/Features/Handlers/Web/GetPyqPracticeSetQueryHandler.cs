using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Web
{

    public record SubjectCountDto(Guid? SubjectId, string Name, int QuestionCount);

    public record YearCountDto(int Year, int QuestionCount);

    public record PyqBankDto( IReadOnlyList<SubjectCountDto> Subjects,  IReadOnlyList<YearCountDto> Years,  int TotalQuestions);
     
    public record PracticeQuestionDto(
        int Id,
        string Text,
        string OptionA,
        string OptionB,
        string OptionC,
        string OptionD,
        int? Year,
        string Subject);

    public record PracticeSetDto(string Title, IReadOnlyList<PracticeQuestionDto> Questions);

    public record AnswerInput(int QuestionId, string? Selected);

    public record QuestionResultDto(
        int QuestionId,
        string? Selected,
        string Correct,
        bool IsCorrect,
        string? Explanation);

    public record PracticeResultDto(
        int Total,
        int Correct,
        int Wrong,
        int Skipped,
        IReadOnlyList<QuestionResultDto> Items);
     

    public record GetPyqBankQuery : IRequest<PyqBankDto>;

    public class GetPyqBankQueryHandler : IRequestHandler<GetPyqBankQuery, PyqBankDto>
    {
        private readonly IRepository<Question> _questionRepository; 
        public GetPyqBankQueryHandler(IRepository<Question> questionRepository) => _questionRepository = questionRepository;

        public async Task<PyqBankDto> Handle(GetPyqBankQuery request, CancellationToken ct)
        {
            var pyq =   _questionRepository.Entity().Include(x=>x.Subject).AsNoTracking().Where(q => q.IsPYQ && q.IsActive).ToList();

            var subjects =   pyq
                .GroupBy(q => new { q.SubjectId, q.Subject?.SubjectName })
                .Select(g => new SubjectCountDto(g.Key.SubjectId, g.Key.SubjectName ?? "", g.Count()))
                .OrderBy(s => s.Name)
                .ToList();

            var years =   pyq
                .Where(q => q.PYQYear != null)
                .GroupBy(q => q.PYQYear!.Value)
                .Select(g => new YearCountDto(g.Key, g.Count()))
                .OrderByDescending(y => y.Year)
                .ToList();

            var total =   pyq.Count( );

            return new PyqBankDto(subjects, years, total);
        }
    }

     

    public record GetPyqPracticeSetQuery(Guid? SubjectId, int? Year, int Count)
        : IRequest<PracticeSetDto?>;

    public class GetPyqPracticeSetQueryHandler
        : IRequestHandler<GetPyqPracticeSetQuery, PracticeSetDto?>
    {
        private readonly IRepository<Question> _questionRepository;
        public GetPyqPracticeSetQueryHandler(IRepository<Question> questionRepository) => _questionRepository = questionRepository; 

        public async Task<PracticeSetDto?> Handle(GetPyqPracticeSetQuery r, CancellationToken ct)
        {
            var query = _questionRepository.Entity().AsNoTracking().Where(q => q.IsPYQ);

            if (r.SubjectId.HasValue) query = query.Where(q => q.SubjectId == r.SubjectId);
            if (r.Year.HasValue) query = query.Where(q => q.PYQYear == r.Year);

            // Guid.NewGuid() -> SQL Server me NEWID() (random order)
            var questions = await query
                .OrderBy(q => Guid.NewGuid())
                .Take(r.Count)
                .Select(q => new PracticeQuestionDto(
                    q.Id, q.QuestionText,
                    q.OptionA, q.OptionB, q.OptionC, q.OptionD,
                    q.PYQYear, q.Subject.Name))
                .ToListAsync(ct);

            if (questions.Count == 0) return null;

            string title;
            if (r.SubjectId.HasValue)
            {
                var name = await _db.Subjects.AsNoTracking()
                    .Where(s => s.Id == r.SubjectId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync(ct);
                title = $"{name} PYQ Practice";
            }
            else if (r.Year.HasValue)
            {
                title = $"PYQ Practice {r.Year}";
            }
            else
            {
                title = "PYQ Practice";
            }

            return new PracticeSetDto(title, questions);
        }
    }


    // =========================== QUERY 3 =================================
    // Answers check karo (kuch save nahi hota, isliye ye query hai, command nahi)

    public record EvaluatePyqAnswersQuery(IReadOnlyList<AnswerInput> Answers)
        : IRequest<PracticeResultDto>;

    public class EvaluatePyqAnswersQueryHandler
        : IRequestHandler<EvaluatePyqAnswersQuery, PracticeResultDto>
    {
        private readonly AppDbContext _db;
        public EvaluatePyqAnswersQueryHandler(AppDbContext db) => _db = db;

        public async Task<PracticeResultDto> Handle(EvaluatePyqAnswersQuery r, CancellationToken ct)
        {
            var ids = r.Answers.Select(a => a.QuestionId).Distinct().ToList();

            var keys = await _db.Questions.AsNoTracking()
                .Where(q => q.IsPyq && ids.Contains(q.Id))
                .Select(q => new { q.Id, q.CorrectOption, q.Explanation })
                .ToDictionaryAsync(q => q.Id, ct);

            var items = new List<QuestionResultDto>();
            int correct = 0, wrong = 0, skipped = 0;

            foreach (var a in r.Answers.DistinctBy(a => a.QuestionId))
            {
                if (!keys.TryGetValue(a.QuestionId, out var key)) continue;

                var selected = string.IsNullOrWhiteSpace(a.Selected)
                    ? null
                    : a.Selected.Trim().ToUpperInvariant();

                var isCorrect = selected != null &&
                    string.Equals(selected, key.CorrectOption, StringComparison.OrdinalIgnoreCase);

                if (selected == null) skipped++;
                else if (isCorrect) correct++;
                else wrong++;

                items.Add(new QuestionResultDto(a.QuestionId, selected, key.CorrectOption, isCorrect, key.Explanation));
            }

            return new PracticeResultDto(items.Count, correct, wrong, skipped, items);
        }
    }
}
