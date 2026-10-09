using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
            try
            {
                if (request == null || request.Answers == null || !request.Answers.Any())
                {
                    return ResponseDto<TopicTestResultDto>.Failure("Submit karne ke liye koi answers receive nahi hue.");
                }

                var questionIds = request.Answers.Select(a => a.QuestionId).Distinct().ToList();

                var questions = await _questionRepo.Query()
                    .AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q => questionIds.Contains(q.QuestionId))
                    .ToListAsync(cancellationToken);

                if (!questions.Any())
                {
                    return ResponseDto<TopicTestResultDto>.Failure("Tests ke questions Database me nahi mile.");
                }

                int totalQuestions = questions.Count;
                int attemptedQuestions = 0;
                int correctAnswers = 0;
                int wrongAnswers = 0;
                int skippedQuestions = 0;
                decimal totalMarks = 0;
                decimal scoreObtained = 0;

                var now = DateTime.Now;

                var attempt = new TestAttempt
                {
                    AttemptId = Guid.NewGuid(),
                    TestId = request.TestId ?? Guid.Empty,
                    UserId = request.UserId,
                    StartedAt = now.AddSeconds(-Math.Abs(request.TimeTakenSeconds)),
                    SubmittedAt = now,
                    Status = "Completed",
                    TotalQuestions = totalQuestions,
                    TimeTakenSeconds = request.TimeTakenSeconds
                };

                var attemptAnswers = new List<TestAttemptAnswer>();
                var newMistakes = new List<QuestionMistake>();

                var existingMistakeQuestionIds = await _mistakeRepo.Query()
                    .AsNoTracking()
                    .Where(m => m.UserId == request.UserId && questionIds.Contains(m.QuestionId))
                    .Select(m => m.QuestionId)
                    .ToListAsync(cancellationToken);

                foreach (var userAns in request.Answers)
                {
                    var q = questions.FirstOrDefault(x => x.QuestionId == userAns.QuestionId);
                    if (q == null) continue;

                    decimal questionMarks = q.Marks > 0 ? q.Marks : 1.0m;
                    decimal negativeMarks = Math.Abs(q.NegativeMarks);
                    totalMarks += questionMarks;
                     
                    Guid? resolvedOptionId = userAns.SelectedOptionId;

                    if (!resolvedOptionId.HasValue && userAns.SelectedOptionNumber.HasValue)
                    {
                        var optionsList = q.Options.OrderBy(o => o.DisplayOrder).ToList();

                        var selectedOpt = optionsList.FirstOrDefault(o => o.DisplayOrder == userAns.SelectedOptionNumber.Value)
                                       ?? (userAns.SelectedOptionNumber.Value <= optionsList.Count
                                            ? optionsList[userAns.SelectedOptionNumber.Value - 1]
                                            : null);

                        if (selectedOpt != null)
                        {
                            resolvedOptionId = selectedOpt.OptionId;
                        }
                    }

                    var correctOption = q.Options.FirstOrDefault(o => o.IsCorrect);
                    bool isCorrect = false;
                    decimal marksObtained = 0;

                    if (userAns.IsAnswered && resolvedOptionId.HasValue)
                    {
                        attemptedQuestions++;

                        if (correctOption != null && resolvedOptionId.Value == correctOption.OptionId)
                        {
                            isCorrect = true;
                            correctAnswers++;
                            marksObtained = questionMarks;
                        }
                        else
                        {
                            wrongAnswers++;
                            marksObtained = -negativeMarks;

                            if (!existingMistakeQuestionIds.Contains(q.QuestionId) && !newMistakes.Any(m => m.QuestionId == q.QuestionId))
                            {
                                newMistakes.Add(new QuestionMistake
                                {
                                    UserId = request.UserId,
                                    QuestionId = q.QuestionId,
                                    LastWrongAt = now
                                });
                            }
                        }
                    }
                    else
                    {
                        skippedQuestions++;
                    }

                    scoreObtained += marksObtained;
                     
                    attemptAnswers.Add(new TestAttemptAnswer
                    {
                        AttemptAnswerId = Guid.NewGuid(),
                        AttemptId = attempt.AttemptId,
                        QuestionId = q.QuestionId,
                        SelectedOptionId = resolvedOptionId,  
                        IsAnswered = userAns.IsAnswered,
                        IsMarkedForReview = userAns.IsMarkedForReview,
                        IsCorrect = userAns.IsAnswered ? isCorrect : null,
                        MarksObtained = marksObtained,
                        AnsweredAt = userAns.IsAnswered ? now : null
                    });
                }

                attempt.AttemptedQuestions = attemptedQuestions;
                attempt.CorrectAnswers = correctAnswers;
                attempt.WrongAnswers = wrongAnswers;
                attempt.SkippedQuestions = skippedQuestions;
                attempt.TotalMarks = totalMarks;
                attempt.Score = scoreObtained;
                attempt.Accuracy = attemptedQuestions > 0
                    ? Math.Round(((decimal)correctAnswers / attemptedQuestions) * 100, 2)
                    : 0;

                await _testAttemptRepo.AddAsync(attempt);
                await _testAttemptAnswerRepo.AddRangeAsync(attemptAnswers);

                if (newMistakes.Any())
                {
                    await _mistakeRepo.AddRangeAsync(newMistakes);
                }

                await _testAttemptRepo.SaveChangesAsync();

                var result = new TopicTestResultDto
                {
                    TotalQuestions = totalQuestions,
                    AttemptedQuestions = attemptedQuestions,
                    CorrectAnswers = correctAnswers,
                    WrongAnswers = wrongAnswers,
                    SkippedQuestions = skippedQuestions,
                    TotalMarks = totalMarks,
                    Score = scoreObtained,
                    Accuracy = attempt.Accuracy
                };

                return ResponseDto<TopicTestResultDto>.Success(result, "Test attempt successfully submit aur save ho gaya.");
            }
            catch (DbUpdateException dbEx)
            { 
                return ResponseDto<TopicTestResultDto>.Failure($"Database Save Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (OperationCanceledException)
            { 
                return ResponseDto<TopicTestResultDto>.Failure("Request canceled ho gayi.");
            }
            catch (Exception ex)
            { 
                return ResponseDto<TopicTestResultDto>.Failure($"Test submit karne me error aaya: {ex.Message}");
            }
        }
    }
}