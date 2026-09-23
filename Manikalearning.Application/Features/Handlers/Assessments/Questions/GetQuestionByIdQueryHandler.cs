using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, ResponseDto<QuestionDto>>
    {
        private readonly IRepository<Question> _questionRepo;

        public GetQuestionByIdQueryHandler(IRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<ResponseDto<QuestionDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
        {
            var q = await _questionRepo.Entity()
                .AsNoTracking()
                .Include(x => x.Options)
                .FirstOrDefaultAsync(x => x.QuestionId == request.QuestionId, cancellationToken);

            if (q == null)
                return ResponseDto<QuestionDto>.Failure("Question not found.");

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
            };

            return ResponseDto<QuestionDto>.Success(dto);
        }
    }
}
