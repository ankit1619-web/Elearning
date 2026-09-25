using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Tests;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Tests
{
    public class GetMappedTestQuestionsQueryHandler : IRequestHandler<GetMappedTestQuestionsQuery, ResponseDto<List<TestQuestionDto>>>
    {
        private readonly IRepository<TestQuestion> _testQuestionRepo;
        private readonly ILogger<GetMappedTestQuestionsQueryHandler> _logger;

        public GetMappedTestQuestionsQueryHandler(
            IRepository<TestQuestion> testQuestionRepo,
            ILogger<GetMappedTestQuestionsQueryHandler> logger)
        {
            _testQuestionRepo = testQuestionRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<List<TestQuestionDto>>> Handle(GetMappedTestQuestionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var mappedQuestions = _testQuestionRepo.Entity().AsNoTracking()
                    .Where(tq => tq.TestId == request.TestId)
                    .OrderBy(tq => tq.QuestionOrder)
                    .Select(tq => new TestQuestionDto
                    {
                        QuestionId = tq.QuestionId,
                        QuestionText = tq.Question != null ? tq.Question.QuestionText : string.Empty,
                        SubjectName = tq.Question != null && tq.Question.Subject != null ? tq.Question.Subject.SubjectName : null,
                        TopicName = tq.Question != null && tq.Question.Topic != null ? tq.Question.Topic.TopicName : null,
                        Marks = tq.Marks,
                        NegativeMarks = tq.Question != null ? tq.Question.NegativeMarks : 0,
                        DisplayOrder = tq.QuestionOrder
                    }).ToList();

                _logger.LogInformation("Fetched {Count} mapped questions for Test ID {TestId}.", mappedQuestions.Count, request.TestId);
                return ResponseDto<List<TestQuestionDto>>.Success(mappedQuestions, "Mapped questions retrieved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching mapped questions for Test ID {TestId}.", request.TestId);
                return ResponseDto<List<TestQuestionDto>>.Failure("An error occurred while retrieving test questions.");
            }
        }
    }
}
