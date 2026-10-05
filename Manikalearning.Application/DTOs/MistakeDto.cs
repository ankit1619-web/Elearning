namespace Manikalearning.Application.DTOs
{
    public class MistakeDto
    {
        public Guid MistakeId { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string SelectedAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string? Explanation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
