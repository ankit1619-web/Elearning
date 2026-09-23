namespace Manikalearning.Application.DTOs
{ 
    public class TopicDto
    {
        public Guid TopicId { get; set; }
        public Guid SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
