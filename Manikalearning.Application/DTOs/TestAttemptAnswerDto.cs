namespace Manikalearning.Application.DTOs
{
    public class TestAttemptAnswerDto
    {
        public Guid AttemptAnswerId { get; set; }

        public Guid AttemptId { get; set; }

        public Guid QuestionId { get; set; }

        public Guid? SelectedOptionId { get; set; }

        public string? AnswerText { get; set; }

        public bool IsAnswered { get; set; }

        public bool IsMarkedForReview { get; set; }

        public bool IsCorrect { get; set; }

        public decimal MarksObtained { get; set; }

        public DateTime? AnsweredAt { get; set; }
    }
}
