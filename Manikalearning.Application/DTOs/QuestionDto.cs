using Manikalearning.Domain.Enums;

namespace Manikalearning.Application.DTOs
{
    public class QuestionDto
    {
        public Guid QuestionId { get; set; }

        public Guid? ExamId { get; set; }

        public Guid? PaperId { get; set; }

        public Guid? SubjectId { get; set; }

        public Guid? SectionId { get; set; }

        public Guid? TopicId { get; set; }

        public Guid? SubTopicId { get; set; }

        public Guid? PassageId { get; set; }

        public QuestionType QuestionType { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? Explanation { get; set; }

        public string? ShortExplanation { get; set; }
         
        public DifficultyLevel Difficulty { get; set; } 

        public decimal Marks { get; set; }

        public decimal NegativeMarks { get; set; }

        public bool IsPYQ { get; set; }

        public int? PYQYear { get; set; }

        public string? PYQSession { get; set; }

        public string? SourceName { get; set; }

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<QuestionOptionDto> Options { get; set; }
            = new();

    }
}
