namespace Manikalearning.Application.DTOs
{
    public class SubTopicDto
    {
        public Guid SubTopicId { get; set; }

        public Guid TopicId { get; set; }

        public string SubTopicName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
