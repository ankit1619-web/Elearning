namespace Manikalearning.Application.DTOs
{ 
    public class SectionDto
    {
        public Guid SectionId { get; set; }
        public Guid SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
