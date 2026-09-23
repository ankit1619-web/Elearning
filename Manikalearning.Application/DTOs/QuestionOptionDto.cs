namespace Manikalearning.Application.DTOs
{
    public class QuestionOptionDto
    {
        public Guid OptionId { get; set; }

        public Guid QuestionId { get; set; }

        public string OptionLabel { get; set; } = string.Empty;

        public string OptionText { get; set; } = string.Empty;
        public string OptionTextHindi { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public int DisplayOrder { get; set; }
    }
}
