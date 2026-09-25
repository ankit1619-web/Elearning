using Manikalearning.Domain.Enums;

namespace Manikalearning.Application.DTOs
{
    public class TestDto
    {
        public Guid TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public Guid? ExamId { get; set; }
        public Guid? PaperId { get; set; }
        public Guid? SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public Guid? SectionId { get; set; }
        public string? SectionName { get; set; }
        public Guid? TopicId { get; set; }
        public string? TopicName { get; set; }
        public string TestType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal NegativeMarking { get; set; }
        public bool IsRandom { get; set; }
        public bool ShowResultImmediately { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TestQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? SubjectName { get; set; }
        public string? TopicName { get; set; }
        public decimal Marks { get; set; }
        public decimal NegativeMarks { get; set; }
        public int DisplayOrder { get; set; }
    }
     
    public class TestQuestionItemDto
    {
        public Guid QuestionId { get; set; }
        public int DisplayOrder { get; set; }
        public decimal Marks { get; set; }
    }
}
