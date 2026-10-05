using Manikalearning.Application.Features.Commands.Students;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Students
{
    public class SubmitTestAttemptCommandHandler : IRequestHandler<SubmitTestAttemptCommand, TestResultDto>
    {
        private readonly IRepository<Test> _testRepo;
        private readonly IRepository<TestAttempt> _attemptRepo;
        private readonly IRepository<QuestionMistake> _mistakeRepo;

        public SubmitTestAttemptCommandHandler(
            IRepository<Test> testRepo,
            IRepository<TestAttempt> attemptRepo,
            IRepository<QuestionMistake> mistakeRepo)
        {
            _testRepo = testRepo;
            _attemptRepo = attemptRepo;
            _mistakeRepo = mistakeRepo;
        }

        public async Task<TestResultDto> Handle(SubmitTestAttemptCommand request, CancellationToken cancellationToken)
        {
            var test = await _testRepo.Query()
                .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                .FirstOrDefaultAsync(t => t.TestId == request.TestId, cancellationToken);

            if (test == null) throw new KeyNotFoundException("Test not found");

            int correctCount = 0;
            int wrongCount = 0;
            var mistakesToAdd = new List<QuestionMistake>();

            foreach (var tq in test.TestQuestions)
            {
                var q = tq.Question;
                request.SelectedAnswers.TryGetValue(q.QuestionId, out var studentAns);

                if (!string.IsNullOrEmpty(studentAns))
                {
                    if (studentAns.Trim().Equals(q.Options.FirstOrDefault(x => x.IsCorrect)?.OptionText?.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        correctCount++;
                    }
                    else
                    {
                        wrongCount++; 
                        mistakesToAdd.Add(new QuestionMistake
                        {
                            MistakeId = Guid.NewGuid(),
                            UserId = request.UserId,
                            QuestionId = q.QuestionId,
                           // SelectedAnswer = studentAns,
                            LastWrongAt = DateTime.UtcNow
                        });
                    }
                }
            }

            var attempt = new TestAttempt
            {
                AttemptId = Guid.NewGuid(),
                UserId = request.UserId,
                TestId = request.TestId,
                Score = correctCount,
                TotalQuestions = test.TestQuestions.Count,
                TimeTakenSeconds = request.TimeTakenSeconds,
                StartedAt = DateTime.UtcNow
            };

            await _attemptRepo.AddAsync(attempt);

            if (mistakesToAdd.Any())
            {
                await _mistakeRepo.AddRangeAsync(mistakesToAdd);
            }

            return new TestResultDto
            {
                AttemptId = attempt.AttemptId,
                Score = correctCount,
                TotalQuestions = attempt.TotalQuestions,
                WrongCount = wrongCount
            };
        }
    }
}
