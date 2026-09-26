using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.TestAttempts;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using Manikalearning.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.TestAttempts
{
    public class GetTestAttemptDetailQueryHandler : IRequestHandler<GetTestAttemptDetailQuery, TestAttemptDetailDto?>
    {
        private readonly IRepository<TestAttempt> _attemptRepo;
        private readonly IRepository<TestAttemptAnswer> _answerRepo;

        public GetTestAttemptDetailQueryHandler(IRepository<TestAttempt> attemptRepo, IRepository<TestAttemptAnswer> answerRepo)
        {
            _attemptRepo = attemptRepo;
            _answerRepo = answerRepo;
        }

        public async Task<TestAttemptDetailDto?> Handle(GetTestAttemptDetailQuery request, CancellationToken cancellationToken)
        {
            // 1. Fetch main attempt details with Test & User
            var attempt = await _attemptRepo.Entity()
                .Include(a => a.Test)
                .Include(a => a.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId, cancellationToken);

            if (attempt == null) return null;
             
            var answers = await _answerRepo.Entity()
                .Include(ans => ans.Question)
                    .ThenInclude(q => q.Options)
                .Where(ans => ans.AttemptId == request.AttemptId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
             
            return new TestAttemptDetailDto
            {
                AttemptId = attempt.AttemptId,
                TestId = attempt.TestId,
                TestTitle = attempt.Test != null ? attempt.Test.TestName : "N/A",
                UserId = attempt.UserId,
                UserName = attempt.User != null ? attempt.User.UserName! : "Student",
                UserEmail = attempt.User != null ? attempt.User.Email! : "",
                StartedAt = attempt.StartedAt,
                SubmittedAt = attempt.SubmittedAt,
                Status = attempt.Status ?? "In Progress",
                TotalQuestions = attempt.TotalQuestions,
                AttemptedQuestions = attempt.AttemptedQuestions,
                CorrectAnswers = attempt.CorrectAnswers,
                WrongAnswers = attempt.WrongAnswers,
                SkippedQuestions = attempt.SkippedQuestions,
                TotalMarks = attempt.TotalMarks,
                Score = attempt.Score,
                Accuracy = attempt.Accuracy,
                TimeTakenSeconds = attempt.TimeTakenSeconds,
                Answers = answers.Select(ans =>
                {
                    var qType = ans.Question != null ? ans.Question.QuestionType : QuestionType.SingleChoice;

                    return new TestAttemptAnswerDetailDto
                    {
                        AttemptAnswerId = ans.AttemptAnswerId,
                        QuestionId = ans.QuestionId,
                        QuestionText = ans.Question != null ? ans.Question.QuestionText : "",
                         
                        QuestionType = qType,
                        QuestionTypeName = qType.GetDisplayName(),

                        SelectedOptionId = ans.SelectedOptionId,
                        SelectedOptionText = ans.Question?.Options.FirstOrDefault(o => o.OptionId == ans.SelectedOptionId)?.OptionText ?? "Not Answered",
                        CorrectOptionText = ans.Question?.Options.FirstOrDefault(o => o.IsCorrect)?.OptionText ?? "N/A",
                        IsAnswered = ans.IsAnswered,
                        IsMarkedForReview = ans.IsMarkedForReview,
                        IsCorrect = ans.IsCorrect,
                        MarksObtained = ans.MarksObtained,
                        AnsweredAt = ans.AnsweredAt,
                        Explanation = ans.Question != null ? ans.Question.Explanation !: ""
                    };
                }).ToList()
            };
        }
    }
}
