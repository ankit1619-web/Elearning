using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class GetTestByIdQueryHandler : IRequestHandler<GetTestByIdQuery, ResponseDto<TestDto>>
    {
        private readonly IRepository<Test> _testRepo;
        private readonly ILogger<GetTestByIdQueryHandler> _logger;

        public GetTestByIdQueryHandler(
            IRepository<Test> testRepo,
            ILogger<GetTestByIdQueryHandler> logger)
        {
            _testRepo = testRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<TestDto>> Handle(GetTestByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var t = await _testRepo.GetByIdAsync(request.Id);
                if (t == null)
                {
                    _logger.LogWarning("Test with ID {Id} was not found.", request.Id);
                    return ResponseDto<TestDto>.Failure("Test details not found.");
                }

                var dto = new TestDto
                {
                    TestId = t.TestId,
                    TestName = t.TestName,
                    ExamId = t.ExamId,
                    PaperId = t.PaperId,
                    SubjectId = t.SubjectId,
                    //SubjectName = t.Subject?.Name,
                    SectionId = t.SectionId,
                   // SectionName = t.Section?.Name,
                    TopicId = t.TopicId,
                   // TopicName = t.Topic?.Name,
                    TestType = t.TestType,
                    DurationMinutes = t.DurationMinutes,
                    TotalQuestions = t.TotalQuestions,
                    TotalMarks = t.TotalMarks,
                    NegativeMarking = t.NegativeMarking,
                    IsRandom = t.IsRandom,
                    ShowResultImmediately = t.ShowResultImmediately,
                    IsPublished = t.IsPublished,
                    CreatedAt = t.CreatedAt
                };

                _logger.LogInformation("Retrieved details for Test ID {Id}.", request.Id);
                return ResponseDto<TestDto>.Success(dto, "Test details retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching Test with ID {Id}.", request.Id);
                return ResponseDto<TestDto>.Failure("An error occurred while retrieving test details.");
            }
        }
    }
}
