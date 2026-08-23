namespace Manikalearning.Application.DTOs
{
    public class TopicDto
    {
        public Guid TopicId { get; set; }

        public Guid SectionId { get; set; }

        public string TopicName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }
    }
}
