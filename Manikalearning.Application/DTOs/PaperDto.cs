namespace Manikalearning.Application.DTOs
{ 
    public class PaperDto
    {
        public Guid PaperId { get; set; }
        public Guid ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string PaperName { get; set; } = string.Empty;
        public string PaperCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
