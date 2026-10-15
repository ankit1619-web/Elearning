using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
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
                    return ResponseDto<TopicTestResultDto>.Failure("No answers were received for submission.");
                }

                var questionIds = request.Answers.Select(a => a.QuestionId).Distinct().ToList();

                var questions = await _questionRepo.Query()
                    .AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q => questionIds.Contains(q.QuestionId))
                    .ToListAsync(cancellationToken);

                if (!questions.Any())
                {
                    return ResponseDto<TopicTestResultDto>.Failure("Test questions were not found in the database.");
                }

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
                    TestId = request.TestId,                                        
                    UserId = request.UserId,
                    StartedAt = now.AddSeconds(-Math.Abs(request.TimeTakenSeconds)),
                    SubmittedAt = now,
                    Status = "Completed",
                    TotalQuestions = totalQuestions,
                    TimeTakenSeconds = request.TimeTakenSeconds
                };

                var attemptAnswers = new List<TestAttemptAnswer>();
                var newMistakes = new List<QuestionMistake>();
                 
                var existingMistakes = await _mistakeRepo.Query()
                    .Where(m => m.UserId == request.UserId && questionIds.Contains(m.QuestionId))
                    .ToListAsync(cancellationToken);
                 
                foreach (var userAns in request.Answers.GroupBy(a => a.QuestionId).Select(g => g.Last()))
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
                        var n = userAns.SelectedOptionNumber.Value;  
                        if (n >= 1 && n <= optionsList.Count) resolvedOptionId = optionsList[n - 1].OptionId;
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
                             
                            foreach (var old in existingMistakes.Where(m => m.QuestionId == q.QuestionId && !m.IsResolved))
                                old.IsResolved = true;
                        }
                        else
                        {
                            wrongAnswers++;
                            marksObtained = -negativeMarks;

                            var old = existingMistakes.FirstOrDefault(m => m.QuestionId == q.QuestionId);
                            if (old != null)
                            {
                                old.WrongCount++;                               
                                old.LastWrongAt = now;
                                old.IsResolved = false;
                            }
                            else if (!newMistakes.Any(m => m.QuestionId == q.QuestionId))
                            {
                                newMistakes.Add(new QuestionMistake
                                {
                                    MistakeId = Guid.NewGuid(),
                                    UserId = request.UserId,
                                    QuestionId = q.QuestionId,
                                    WrongCount = 1,                         
                                    LastWrongAt = now,
                                    IsResolved = false
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

                return ResponseDto<TopicTestResultDto>.Success(result, "Test attempt successfully submitted and saved.");
            }
            catch (DbUpdateException dbEx)
            {
                return ResponseDto<TopicTestResultDto>.Failure($"Database Save Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (OperationCanceledException)
            {
                return ResponseDto<TopicTestResultDto>.Failure("The request was canceled.");
            }
            catch (Exception ex)
            {
                return ResponseDto<TopicTestResultDto>.Failure($"An error occurred while submitting the test: {ex.Message}");
            }
        }
    }
}
