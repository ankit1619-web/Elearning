namespace Manikalearning.Application.DTOs
{
    public class PyqTestState
    {
        public Guid SectionId { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public int DurationSeconds { get; set; }
        public List<PyqAnswerState> Answers { get; set; } = new();
    }

    public class PyqAnswerState
    {
        public Guid QuestionId { get; set; }
        public int? Selected { get; set; }
        public int Status { get; set; }          // 0 not visited, 1 not answered, 2 answered, 3 marked
        public bool Bookmarked { get; set; }
    }

    public class PyqQuestionVm
    {
        public TopicPyqDto Question { get; set; } = default!;
        public int Index { get; set; }
        public int Total { get; set; }
        public int? Selected { get; set; }
        public bool Bookmarked { get; set; }
        public List<int> Statuses { get; set; } = new();
        public int SecondsLeft { get; set; }
        public bool IsPreviewOnly { get; set; }
        public string? BannerMessage { get; set; }
    }

    public class PyqReviewVm
    {
        public int Total { get; set; }
        public int Answered { get; set; }
        public int NotAnswered { get; set; }
        public int Marked { get; set; }
        public int SecondsLeft { get; set; }
    }

    public class PyqResultVm
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public decimal Score { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Accuracy { get; set; }
        public bool AutoSubmitted { get; set; }
    }
    public class PyqAnswerDto
    {
        public Guid QuestionId { get; set; }
        public int? SelectedOptionNumber { get; set; }   
        public bool IsAnswered { get; set; }
        public bool IsMarkedForReview { get; set; }
    }
     
    public class PyqTestResultDto
    {
        public Guid AttemptId { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int SkippedQuestions { get; set; }
        public decimal Score { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal Accuracy { get; set; }            // correct / attempted * 100
    }
}
