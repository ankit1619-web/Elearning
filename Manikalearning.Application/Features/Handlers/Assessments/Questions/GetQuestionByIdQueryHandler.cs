using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, ResponseDto<QuestionDto>>
    {
        private readonly IRepository<Question> _questionRepo;
        private readonly ILogger<GetQuestionByIdQueryHandler> _logger;

        public GetQuestionByIdQueryHandler(
            IRepository<Question> questionRepo,
            ILogger<GetQuestionByIdQueryHandler> logger)
        {
            _questionRepo = questionRepo;
            _logger = logger;
        }

        public async Task<ResponseDto<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Fetch Question with Options from DB
                var q = await _questionRepo.Entity()
                    .AsNoTracking()
                    .Include(x => x.Options)
                    .FirstOrDefaultAsync(x => x.QuestionId == request.QuestionId, cancellationToken);

                if (q == null)
                {
                    _logger.LogWarning("Question with ID: {QuestionId} was not found.", request.QuestionId);
                    return ResponseDto<QuestionDto>.Failure($"Question with ID '{request.QuestionId}' not found.");
                }

                // 2. Map Entity to DTO
                var dto = new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    SubjectId = q.SubjectId,
                    SectionId = q.SectionId,
                    TopicId = q.TopicId,
                    SubTopicId = q.SubTopicId,
                    ExamId = q.ExamId,
                    PaperId = q.PaperId,
                    PassageId = q.PassageId,
                    QuestionType = q.QuestionType,

                    // English & Hindi Fields
                    QuestionText = q.QuestionText,
                    QuestionTextHindi = q.QuestionTextHindi,
                    Explanation = q.Explanation,
                    ExplanationHindi = q.ExplanationHindi,
                    ShortExplanation = q.ShortExplanation,
                    ShortExplanationHindi = q.ShortExplanationHindi,

                    Difficulty = q.Difficulty,
                    Marks = q.Marks,
                    NegativeMarks = q.NegativeMarks,
                    IsPYQ = q.IsPYQ,
                    PYQYear = q.PYQYear,
                    PYQSession = q.PYQSession,
                    SourceName = q.SourceName,
                    IsApproved = q.IsApproved,
                    IsActive = q.IsActive,
                    CreatedAt = q.CreatedAt,

                    // Options Mapping with Null check safety
                    Options = q.Options != null
                        ? q.Options.OrderBy(o => o.DisplayOrder).Select(o => new QuestionOptionDto
                        {
                            OptionId = o.OptionId,
                            QuestionId = o.QuestionId,
                            OptionLabel = o.OptionLabel,
                            OptionText = o.OptionText,
                            OptionTextHindi = o.OptionTextHindi,
                            IsCorrect = o.IsCorrect,
                            DisplayOrder = o.DisplayOrder
                        }).ToList()
                        : new List<QuestionOptionDto>()
                };

                return ResponseDto<QuestionDto>.Success(dto);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Fetch question operation was canceled for QuestionId: {QuestionId}", request.QuestionId);
                return ResponseDto<QuestionDto>.Failure("Operation was canceled.");
            }
            catch (Exception ex)
            {
                // 3. Log Exception details
                _logger.LogError(ex, "An error occurred while fetching question details for ID: {QuestionId}", request.QuestionId);

                // 4. Return Structured Failure Response
                return ResponseDto<QuestionDto>.Failure("An unexpected error occurred while processing your request. Please try again.");
            }
        }
    }
}
