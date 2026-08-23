namespace Manikalearning.Application.DTOs
{
    public class SubjectDto
    {
        public Guid SubjectId { get; set; }

        public Guid PaperId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }
    }
}
