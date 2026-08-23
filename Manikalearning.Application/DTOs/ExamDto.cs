namespace Manikalearning.Application.DTOs
{
    public class ExamDto
    {
        public Guid ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public string? ExamCode { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
