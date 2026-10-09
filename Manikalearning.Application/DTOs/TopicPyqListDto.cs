namespace Manikalearning.Application.DTOs
{
    public class TopicPyqListDto
    {
        public List<TopicPyqDto> Questions { get; set; } = new();
        public bool IsSubscribed { get; set; }
        public bool IsPreviewOnly { get; set; }
        public int TotalAvailableQuestions { get; set; }
        public string? BannerMessage { get; set; }
    }

    public class TopicPyqDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? QuestionTextHindi { get; set; }
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public string? ExplanationHindi { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int? PYQYear { get; set; }
        public string? PYQSession { get; set; }
        public bool IsBookmarked { get; set; }
    }
}
