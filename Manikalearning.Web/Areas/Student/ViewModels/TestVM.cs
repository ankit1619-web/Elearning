using Manikalearning.Domain.Enums;

namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class TestVM
    { 

        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TestType TestType { get; set; }

        public int TotalQuestions { get; set; }

        public decimal TotalMarks { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsAttempted { get; set; }

        public bool IsCompleted { get; set; }

        public decimal? PreviousScore { get; set; }
         

        public Guid AttemptId { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? EndTime { get; set; }

        public int CurrentQuestionNumber { get; set; }

        public int RemainingSeconds { get; set; }


        public List<TestQuestionVM> Questions { get; set; } = new();

         

        public List<TestVM> Items { get; set; } = new();

        public string? Search { get; set; }

        public TestType? FilterType { get; set; }
    }


    public class TestQuestionVM
    {
        public Guid QuestionId { get; set; }

        public int QuestionNumber { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public decimal Marks { get; set; }

        public decimal NegativeMarks { get; set; }

        public Guid? SelectedOptionId { get; set; }

        public bool IsAnswered { get; set; }

        public bool IsMarkedForReview { get; set; }

        public List<TestOptionVM> Options { get; set; } = new();
    }


    public class TestOptionVM
    {
        public Guid OptionId { get; set; }

        public string OptionLabel { get; set; } = string.Empty;

        public string OptionText { get; set; } = string.Empty;
    }
    public class TestResultVM
    {
        public Guid AttemptId { get; set; }

        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public decimal TotalMarks { get; set; }

        public decimal ObtainedMarks { get; set; }

        public decimal Percentage { get; set; }

        public int CorrectAnswers { get; set; }

        public int WrongAnswers { get; set; }

        public int SkippedAnswers { get; set; }

        public TimeSpan TimeTaken { get; set; }

        public int Rank { get; set; }

        public int TotalParticipants { get; set; }

        public List<ResultQuestionVM> Questions { get; set; } = new();
    }


    public class ResultQuestionVM
    {
        public Guid QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public string? SelectedOption { get; set; }

        public string? CorrectOption { get; set; }

        public bool IsCorrect { get; set; }

        public decimal MarksObtained { get; set; }

        public string? Explanation { get; set; }
    }
}
