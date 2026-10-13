using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Pyq;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Pyq
{
    public class SubmitPyqTestCommandHandler : IRequestHandler<SubmitPyqTestCommand, PyqTestResultDto>
    {
        private const decimal MarksPerCorrect = 2m;
        private const decimal NegativePerWrong = 0m;

        private readonly IRepository<Question> _questionRepo;
        private readonly IRepository<QuestionOption> _questionOptionRepo;
        private readonly IRepository<TestAttempt> _testAttemptRepo;
        private readonly IRepository<TestAttemptAnswer> _testAttemptAnswerRepo;
        private readonly IRepository<QuestionMistake> _mistakeRepo;

        public SubmitPyqTestCommandHandler(
            IRepository<Question> questionRepo,
            IRepository<QuestionOption> questionOptionRepo,
            IRepository<TestAttempt> testAttemptRepo,
            IRepository<TestAttemptAnswer> testAttemptAnswerRepo,
            IRepository<QuestionMistake> mistakeRepo)
        {
            _questionRepo = questionRepo;
            _questionOptionRepo = questionOptionRepo;         
            _testAttemptRepo = testAttemptRepo;
            _testAttemptAnswerRepo = testAttemptAnswerRepo;
            _mistakeRepo = mistakeRepo;
        }

        public async Task<PyqTestResultDto> Handle(SubmitPyqTestCommand req, CancellationToken ct)
        {
            if (req.UserId == Guid.Empty) throw new UnauthorizedAccessException("User not found.");
            if (req.Answers == null || req.Answers.Count == 0) throw new ArgumentException("No answers to submit.");

            var answers = req.Answers.GroupBy(a => a.QuestionId).Select(g => g.Last()).ToList();
            var questionIds = answers.Select(a => a.QuestionId).ToList();

            var optionRows = await _questionOptionRepo.Entity()
                .AsNoTracking()
                .Where(o => questionIds.Contains(o.QuestionId))
                .OrderBy(o => o.DisplayOrder)
                .Select(o => new { o.QuestionId, o.OptionId, o.IsCorrect })
                .ToListAsync(ct);

            var optionsByQuestion = optionRows
                .GroupBy(o => o.QuestionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var now = DateTime.UtcNow;
            var attemptId = Guid.NewGuid();
            var rows = new List<TestAttemptAnswer>();
            var wrongQuestionIds = new List<Guid>();
            var correctQuestionIds = new List<Guid>();        
            int correct = 0, wrong = 0, skipped = 0;

            foreach (var a in answers)
            {
                optionsByQuestion.TryGetValue(a.QuestionId, out var opts);

                Guid? selectedOptionId = null;
                bool? isCorrect = null;

                if (a.IsAnswered && a.SelectedOptionNumber is int n && opts != null && n >= 1 && n <= opts.Count)
                {
                    var chosen = opts[n - 1];
                    selectedOptionId = chosen.OptionId;
                    isCorrect = chosen.IsCorrect;
                }

                var answered = selectedOptionId != null;
                decimal marks = 0;

                if (!answered) skipped++;
                else if (isCorrect == true)
                {
                    correct++; marks = MarksPerCorrect;
                    correctQuestionIds.Add(a.QuestionId);       
                }
                else
                {
                    wrong++; marks = -NegativePerWrong;
                    wrongQuestionIds.Add(a.QuestionId);
                }

                rows.Add(new TestAttemptAnswer
                {
                    AttemptAnswerId = Guid.NewGuid(),
                    AttemptId = attemptId,
                    QuestionId = a.QuestionId,
                    SelectedOptionId = selectedOptionId,
                    IsAnswered = answered,
                    IsMarkedForReview = a.IsMarkedForReview,
                    IsCorrect = isCorrect,  
                    MarksObtained = marks,
                    AnsweredAt = answered ? now : null
                });
            }

            var total = answers.Count;
            var attempted = correct + wrong;
            var score = correct * MarksPerCorrect - wrong * NegativePerWrong;
            var totalMarks = total * MarksPerCorrect;
            var accuracy = attempted > 0 ? Math.Round(correct * 100m / attempted, 2) : 0m;

            await _testAttemptRepo.AddAsync(new TestAttempt
            {
                AttemptId = attemptId,
                TestId = null,
                UserId = req.UserId,
                StartedAt = now.AddSeconds(-req.TimeTakenSeconds),
                SubmittedAt = now,
                Status = "Submitted",
                TotalQuestions = total,
                AttemptedQuestions = attempted,
                CorrectAnswers = correct,
                WrongAnswers = wrong,
                SkippedQuestions = skipped,
                TotalMarks = totalMarks,
                Score = score,
                Accuracy = accuracy,
                TimeTakenSeconds = req.TimeTakenSeconds
            });
             
            foreach (var row in rows)
                await _testAttemptAnswerRepo.AddAsync(row);
             
            if (correctQuestionIds.Count > 0)
            {
                var solved = await _mistakeRepo.Entity()
                    .Where(m => m.UserId == req.UserId && !m.IsResolved && correctQuestionIds.Contains(m.QuestionId))
                    .ToListAsync(ct);
                foreach (var m in solved) m.IsResolved = true;
            }
             
            if (wrongQuestionIds.Count > 0)
            {
                var existing = await _mistakeRepo.Entity()
                    .Where(m => m.UserId == req.UserId && wrongQuestionIds.Contains(m.QuestionId))
                    .ToListAsync(ct);

                foreach (var qid in wrongQuestionIds)
                {
                    var m = existing.FirstOrDefault(x => x.QuestionId == qid);

                    if (m == null)
                    {
                        await _mistakeRepo.AddAsync(new QuestionMistake
                        {
                            MistakeId = Guid.NewGuid(),
                            UserId = req.UserId,
                            QuestionId = qid,
                            WrongCount = 1,
                            LastWrongAt = now,
                            IsResolved = false
                        });
                    }
                    else
                    {
                        m.WrongCount++;
                        m.LastWrongAt = now;
                        m.IsResolved = false;
                    }
                }
            }

            await _testAttemptRepo.SaveChangesAsync();

            return new PyqTestResultDto
            {
                AttemptId = attemptId,
                TotalQuestions = total,
                CorrectAnswers = correct,
                WrongAnswers = wrong,
                SkippedQuestions = skipped,
                Score = score,
                TotalMarks = totalMarks,
                Accuracy = accuracy
            };
        }
    }
}