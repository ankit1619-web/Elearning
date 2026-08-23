namespace Manikalearning.Application.DTOs
{
    public class TestAttemptDto
    {
        public Guid AttemptId { get; set; }

        public Guid TestId { get; set; }

        public Guid UserId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public int AttemptNumber { get; set; }

        public decimal TotalMarks { get; set; }

        public decimal ObtainedMarks { get; set; }

        public int CorrectAnswers { get; set; }

        public int WrongAnswers { get; set; }

        public int SkippedAnswers { get; set; }

        public bool IsCompleted { get; set; }
    }
}
