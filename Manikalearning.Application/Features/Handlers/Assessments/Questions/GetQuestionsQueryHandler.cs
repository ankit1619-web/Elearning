using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, ResponseDto<List<QuestionDto>>>
    {
        private readonly IRepository<Question> _questionRepo;

        public GetQuestionsQueryHandler(IRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<ResponseDto<List<QuestionDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
        {
            var query = _questionRepo.Entity()
                .AsNoTracking()
                .Include(q => q.Options)
                .AsQueryable();

            if (request.TopicId.HasValue && request.TopicId.Value != Guid.Empty)
            {
                query = query.Where(q => q.TopicId == request.TopicId.Value);
            }

            if (request.SubTopicId.HasValue && request.SubTopicId.Value != Guid.Empty)
            {
                query = query.Where(q => q.SubTopicId == request.SubTopicId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(q => q.QuestionText.ToLower().Contains(search) ||
                                         (q.SourceName != null && q.SourceName.ToLower().Contains(search)));
            }

            var questions = await query
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new QuestionDto
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
                    QuestionText = q.QuestionText,
                    Explanation = q.Explanation,
                    ShortExplanation = q.ShortExplanation,
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
                    Options = q.Options.OrderBy(o => o.DisplayOrder).Select(o => new QuestionOptionDto
                    {
                        OptionId = o.OptionId,
                        QuestionId = o.QuestionId,
                        OptionLabel = o.OptionLabel,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect,
                        DisplayOrder = o.DisplayOrder
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            return ResponseDto<List<QuestionDto>>.Success(questions, "Questions retrieved successfully.");
        }
    }
}
