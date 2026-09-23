using Manikalearning.Application.DTOs;
using Manikalearning.Application.Features.Commands.Assessments.Questions;
using Manikalearning.Application.Interface;
using Manikalearning.Domain.Entities;
using MediatR;

namespace Manikalearning.Application.Features.Handlers.Assessments.Questions
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, ResponseDto<Guid>>
    {
        private readonly IRepository<Question> _questionRepo;

        public CreateQuestionCommandHandler(IRepository<Question> questionRepo)
        {
            _questionRepo = questionRepo;
        }

        public async Task<ResponseDto<Guid>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var question = new Question
            {
                QuestionId = Guid.NewGuid(),
                SubjectId = request.SubjectId,
                SectionId = request.SectionId,
                TopicId = request.TopicId,
                SubTopicId = request.SubTopicId == Guid.Empty ? null : request.SubTopicId,
                ExamId = request.ExamId == Guid.Empty ? null : request.ExamId,
                PaperId = request.PaperId == Guid.Empty ? null : request.PaperId,
                PassageId = request.PassageId == Guid.Empty ? null : request.PassageId,
                QuestionType = request.QuestionType,
                QuestionText = request.QuestionText,
                QuestionTextHindi = request.QuestionTextHindi,
                Explanation = request.Explanation,
                ExplanationHindi = request.ExplanationHindi,
                ShortExplanation = request.ShortExplanation,
                ShortExplanationHindi = request.ShortExplanationHindi,
                Difficulty = request.Difficulty,
                Marks = request.Marks,
                NegativeMarks = request.NegativeMarks,
                IsPYQ = request.IsPYQ,
                PYQYear = request.IsPYQ ? request.PYQYear : null,
                PYQSession = request.IsPYQ ? request.PYQSession : null,
                SourceName = request.SourceName,
                IsApproved = request.IsApproved,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            int order = 1;
            foreach (var opt in request.Options)
            {
                question.Options.Add(new QuestionOption
                {
                    OptionId = Guid.NewGuid(),
                    QuestionId = question.QuestionId,
                    OptionLabel = opt.OptionLabel,
                    OptionText = opt.OptionText,
                    IsCorrect = opt.IsCorrect,
                    DisplayOrder = order++
                });
            }

            await _questionRepo.AddAsync(question);
            await _questionRepo.SaveChangesAsync();

            return ResponseDto<Guid>.Success(question.QuestionId, "Question created successfully.");
        }
    }
}
