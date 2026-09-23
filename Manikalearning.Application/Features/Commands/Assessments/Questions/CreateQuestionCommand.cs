using Manikalearning.Application.DTOs;
using Manikalearning.Domain.Enums;
using MediatR;

namespace Manikalearning.Application.Features.Commands.Assessments.Questions
{
    public class CreateQuestionCommand : IRequest<ResponseDto<Guid>>
    {
        public Guid? SubjectId { get; set; }
        public Guid? SectionId { get; set; }
        public Guid? TopicId { get; set; }
        public Guid? SubTopicId { get; set; }
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? PassageId { get; set; }
        public QuestionType QuestionType { get; set; } 
        public DifficultyLevel Difficulty { get; set; } 
        public string QuestionText { get; set; } = string.Empty;
        public string? QuestionTextHindi { get; set; }
        public string? Explanation { get; set; }
        public string? ExplanationHindi { get; set; }
        public string? ShortExplanation { get; set; }
        public string? ShortExplanationHindi { get; set; }
        public decimal Marks { get; set; } = 1.0m;
        public decimal NegativeMarks { get; set; } = 0.25m;

        public bool IsPYQ { get; set; }
        public int? PYQYear { get; set; }
        public string? PYQSession { get; set; }
        public string? SourceName { get; set; }

        public bool IsApproved { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public List<QuestionOptionDto> Options { get; set; } = new();
    }
}
