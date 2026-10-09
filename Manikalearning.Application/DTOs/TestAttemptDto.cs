using Manikalearning.Domain.Enums;

namespace Manikalearning.Application.DTOs
{
    public class TestAttemptDto
    {
        public Guid AttemptId { get; set; }
        public Guid? TestId { get; set; }
        public string TestTitle { get; set; } = string.Empty;

        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string Status { get; set; } = "In Progress"; // Completed, In Progress, Reset

        public int TotalQuestions { get; set; }
        public int AttemptedQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int SkippedQuestions { get; set; }

        public decimal TotalMarks { get; set; }
        public decimal Score { get; set; }
        public decimal Accuracy { get; set; }
        public int TimeTakenSeconds { get; set; }
    }
    public class TestAttemptDetailDto : TestAttemptDto
    {
        public List<TestAttemptAnswerDetailDto> Answers { get; set; } = new();
    }

    public class TestAttemptAnswerDetailDto
    {
        public Guid AttemptAnswerId { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty; 
        public QuestionType QuestionType { get; set; }
        public string QuestionTypeName { get; set; } = string.Empty; // Single Choice, True / False etc.
        public Guid? SelectedOptionId { get; set; }
        public string SelectedOptionText { get; set; } = string.Empty;
        public string CorrectOptionText { get; set; } = string.Empty;

        public bool IsAnswered { get; set; }
        public bool IsMarkedForReview { get; set; }
        public bool? IsCorrect { get; set; }
        public decimal MarksObtained { get; set; }
        public DateTime? AnsweredAt { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}
