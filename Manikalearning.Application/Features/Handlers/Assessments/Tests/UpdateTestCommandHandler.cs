using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class UpdateTestCommandHandler : IRequestHandler<UpdateTestCommand, ResponseDto<bool>>
    {
        private readonly IRepository<Test> _testRepo;
        private readonly ILogger<UpdateTestCommandHandler> _logger;

        public UpdateTestCommandHandler(
            IRepository<Test> testRepo,
            ILogger<UpdateTestCommandHandler> logger)
        {
            _testRepo = testRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var test = await _testRepo.GetByIdAsync(request.TestId);
                if (test == null)
                {
                    _logger.LogWarning("Update failed. Test with ID {Id} not found.", request.TestId);
                    return ResponseDto<bool>.Failure("Test record not found.");
                }

                test.TestName = request.TestName;
                test.ExamId = request.ExamId;
                test.PaperId = request.PaperId;
                test.SubjectId = request.SubjectId;
                test.SectionId = request.SectionId;
                test.TopicId = request.TopicId;
                test.TestType = request.TestType;
                test.DurationMinutes = request.DurationMinutes;
                test.TotalQuestions = request.TotalQuestions;
                test.TotalMarks = request.TotalMarks;
                test.NegativeMarking = request.NegativeMarking;
                test.IsRandom = request.IsRandom;
                test.ShowResultImmediately = request.ShowResultImmediately;
                test.IsPublished = request.IsPublished; 

                  _testRepo.Update(test);
                await _testRepo.SaveChangesAsync();

                _logger.LogInformation("Test with ID {Id} updated successfully.", request.TestId);
                return ResponseDto<bool>.Success(true, "Test updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating Test with ID {Id}.", request.TestId);
                return ResponseDto<bool>.Failure("An error occurred while updating Test.");
            }
        }
    }
}
