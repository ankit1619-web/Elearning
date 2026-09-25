using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class SaveTestQuestionsCommandHandler : IRequestHandler<SaveTestQuestionsCommand, ResponseDto<bool>>
    {
        private readonly IRepository<TestQuestion> _testQuestionRepo;
        private readonly IRepository<Test> _testRepo;
        private readonly ILogger<SaveTestQuestionsCommandHandler> _logger;

        public SaveTestQuestionsCommandHandler(
            IRepository<TestQuestion> testQuestionRepo,
            IRepository<Test> testRepo,
            ILogger<SaveTestQuestionsCommandHandler> logger)
        {
            _testQuestionRepo = testQuestionRepo;
            _testRepo = testRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(SaveTestQuestionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingMappings = _testQuestionRepo.Entity()
                    .Where(tq => tq.TestId == request.TestId)
                    .ToList();

                foreach (var oldItem in existingMappings)
                {
                      _testQuestionRepo.Remove(oldItem);
                }

                var newMappings = request.Questions.Select(q => new TestQuestion
                {
                    TestQuestionId = Guid.NewGuid(),
                    TestId = request.TestId,
                    QuestionId = q.QuestionId,
                    QuestionOrder = q.DisplayOrder,
                    Marks = q.Marks 
                }).ToList();

                foreach (var newItem in newMappings)
                {
                    await _testQuestionRepo.AddAsync(newItem);
                }

                var test = await _testRepo.GetByIdAsync(request.TestId);
                if (test != null)
                {
                    test.TotalQuestions = newMappings.Count;
                    test.TotalMarks = newMappings.Sum(m => m.Marks);
                      _testRepo.Update(test);
                }

                await _testQuestionRepo.SaveChangesAsync();

                _logger.LogInformation("Saved {Count} questions for Test ID {TestId}.", newMappings.Count, request.TestId);
                return ResponseDto<bool>.Success(true, "Test questions saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving questions for Test ID {TestId}.", request.TestId);
                return ResponseDto<bool>.Failure("An error occurred while mapping test questions.");
            }
        }
    }
}
