using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Pyq
{
    public class SubmitTopicPyqTestCommandHandler : IRequestHandler<SubmitTopicPyqTestCommand, ResponseDto<TopicTestResultDto>>
    {
        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<TestAttempt> _testAttemptRepo;
        private readonly IRepository<TestAttemptAnswer> _testAttemptAnswerRepo;
        private readonly IRepository<QuestionMistake> _mistakeRepo;

        public SubmitTopicPyqTestCommandHandler(
            IRepository<Question> questionRepo,
            IRepository<TestAttempt> testAttemptRepo,
            IRepository<TestAttemptAnswer> testAttemptAnswerRepo,
            IRepository<QuestionMistake> mistakeRepo)
        {
            _questionRepo = questionRepo;
            _testAttemptRepo = testAttemptRepo;
            _testAttemptAnswerRepo = testAttemptAnswerRepo;
            _mistakeRepo = mistakeRepo;
        }

        public async Task<ResponseDto<TopicTestResultDto>> Handle(SubmitTopicPyqTestCommand request, CancellationToken cancellationToken)
        {
            var questionIds = request.Answers.Select(a => a.QuestionId).ToList();
             
            var questions = await _questionRepo.Query()
                .AsNoTracking()
                .Include(q => q.Options)
                .Where(q => questionIds.Contains(q.QuestionId))
                .ToListAsync(cancellationToken);

            int totalQuestions = questions.Count;
            int attemptedQuestions = 0;
            int correctAnswers = 0;
            int wrongAnswers = 0;
            int skippedQuestions = 0;
            decimal totalMarks = 0;
            decimal scoreObtained = 0;

            var now = DateTime.UtcNow;
             
            var attempt = new TestAttempt
            {
                AttemptId = Guid.NewGuid(),
                TestId = request.TestId ?? Guid.Empty,  
                UserId = request.UserId,
                StartedAt = now.AddSeconds(-request.TimeTakenSeconds),
                SubmittedAt = now,
                Status = "Completed",
                TotalQuestions = totalQuestions,
                TimeTakenSeconds = request.TimeTakenSeconds
            };

            var attemptAnswers = new List<TestAttemptAnswer>();
            var newMistakes = new List<QuestionMistake>();

            foreach (var userAns in request.Answers)
            {
                var q = questions.FirstOrDefault(x => x.QuestionId == userAns.QuestionId);
                if (q == null) continue;

                decimal questionMarks = q.Marks > 0 ? q.Marks : 1.0m;
                decimal negativeMarks = q.NegativeMarks;
                totalMarks += questionMarks;

                var correctOption = q.QuestionOptions.FirstOrDefault(o => o.IsCorrect);
                bool isCorrect = false;
                decimal marksObtained = 0;

                if (userAns.IsAnswered && userAns.SelectedOptionId.HasValue)
                {
                    attemptedQuestions++;
                    if (correctOption != null && userAns.SelectedOptionId.Value == correctOption.OptionId)
                    {
                        isCorrect = true;
                        correctAnswers++;
                        marksObtained = questionMarks;
                    }
                    else
                    {
                        wrongAnswers++;
                        marksObtained = -negativeMarks;

                        // QuestionMistakes record
                        newMistakes.Add(new QuestionMistake
                        {
                            UserId = request.UserId,
                            QuestionId = q.QuestionId,
                            CreatedAt = now
                        });
                    }
                }
                else
                {
                    skippedQuestions++;
                }

                scoreObtained += marksObtained;

                // 2. Prepare TestAttemptAnswer Record
                attemptAnswers.Add(new TestAttemptAnswer
                {
                    AttemptAnswerId = Guid.NewGuid(),
                    AttemptId = attempt.AttemptId,
                    QuestionId = q.QuestionId,
                    SelectedOptionId = userAns.SelectedOptionId,
                    IsAnswered = userAns.IsAnswered,
                    IsMarkedForReview = userAns.IsMarkedForReview,
                    IsCorrect = userAns.IsAnswered ? isCorrect : null,
                    MarksObtained = marksObtained,
                    AnsweredAt = userAns.IsAnswered ? now : null
                });
            }

            // Calculations
            decimal accuracy = attemptedQuestions > 0
                ? Math.Round(((decimal)correctAnswers / attemptedQuestions) * 100, 2)
                : 0;

            attempt.AttemptedQuestions = attemptedQuestions;
            attempt.CorrectAnswers = correctAnswers;
            attempt.WrongAnswers = wrongAnswers;
            attempt.SkippedQuestions = skippedQuestions;
            attempt.TotalMarks = totalMarks;
            attempt.Score = scoreObtained;
            attempt.Accuracy = accuracy;

            // Save Records
            await _testAttemptRepo.AddAsync(attempt, cancellationToken);
            await _testAttemptAnswerRepo.AddRangeAsync(attemptAnswers, cancellationToken);

            if (newMistakes.Any())
            {
                await _mistakeRepo.AddRangeAsync(newMistakes, cancellationToken);
            }

            var result = new TopicTestResultDto
            {
                TotalQuestions = totalQuestions,
                AttemptedQuestions = attemptedQuestions,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                SkippedQuestions = skippedQuestions,
                TotalMarks = totalMarks,
                Score = scoreObtained,
                Accuracy = accuracy
            };

            return ResponseDto<TopicTestResultDto>.Success(result, "Test attempt saved successfully.");
        }
    }
}
