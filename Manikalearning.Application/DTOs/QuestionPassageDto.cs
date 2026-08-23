namespace Manikalearning.Application.DTOs
{
    public class QuestionPassageDto
    {
        public Guid PassageId { get; set; }

        public string? Title { get; set; }

        public string PassageText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
