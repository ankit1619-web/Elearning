using FluentValidation;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.Features.Handlers.Assessments.Pyq
{
    public record GradePyqTestQuery(List<UserAnswerDto> Answers) : IRequest<TopicTestResultDto>;

    public class GradePyqTestQueryValidator : AbstractValidator<GradePyqTestQuery>
    {
        public GradePyqTestQueryValidator()
        {
            RuleFor(x => x.Answers)
                .NotNull()
                .Must(a => a.Count is >= 1 and <= 300)
                .WithMessage("Invalid number of answers.");

            RuleForEach(x => x.Answers).SetValidator(new UserAnswerDtoValidator());
        }
    }
    public class UserAnswerDtoValidator : AbstractValidator<UserAnswerDto>
    {
        public UserAnswerDtoValidator()
        {
            RuleFor(x => x.QuestionId).NotEmpty().WithMessage("Question ID is required.");
        }
    }

    public class GradePyqTestQueryHandler : IRequestHandler<GradePyqTestQuery, TopicTestResultDto>
    {
        private readonly IRepository<Question> _questions;
        public GradePyqTestQueryHandler(IRepository<Question> questions) => _questions = questions;

        public async Task<TopicTestResultDto> Handle(GradePyqTestQuery req, CancellationToken ct)
        {
            var answers = req.Answers.GroupBy(a => a.QuestionId).Select(g => g.Last()).ToList();    
            var ids = answers.Select(a => a.QuestionId).ToList();

            var questions = await _questions.Query().AsNoTracking().Include(q => q.Options)
                .Where(q => ids.Contains(q.QuestionId)).ToListAsync(ct);

            int attempted = 0, correct = 0, wrong = 0, skipped = 0;
            decimal totalMarks = 0, score = 0;

            foreach (var a in answers)
            {
                var q = questions.FirstOrDefault(x => x.QuestionId == a.QuestionId);
                if (q == null) continue;

                var marks = q.Marks > 0 ? q.Marks : 1.0m;
                totalMarks += marks;

                Guid? chosen = a.SelectedOptionId;
                if (!chosen.HasValue && a.SelectedOptionNumber.HasValue)
                {
                    var list = q.Options.OrderBy(o => o.DisplayOrder).ToList();
                    var n = a.SelectedOptionNumber.Value;                       
                    if (n >= 1 && n <= list.Count) chosen = list[n - 1].OptionId;
                }

                if (a.IsAnswered && chosen.HasValue)
                {
                    attempted++;
                    var right = q.Options.FirstOrDefault(o => o.IsCorrect);
                    if (right != null && right.OptionId == chosen.Value) { correct++; score += marks; }
                    else { wrong++; score -= Math.Abs(q.NegativeMarks); }
                }
                else skipped++;
            }

            return new TopicTestResultDto
            {
                TotalQuestions = questions.Count,
                AttemptedQuestions = attempted,
                CorrectAnswers = correct,
                WrongAnswers = wrong,
                SkippedQuestions = skipped,
                TotalMarks = totalMarks,
                Score = score,
                Accuracy = attempted > 0 ? Math.Round(correct * 100m / attempted, 2) : 0
            };
        }
    }
}
