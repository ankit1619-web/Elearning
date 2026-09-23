namespace Manikalearning.Application.DTOs
{ 
    public class SubTopicDto
    {
        public Guid SubTopicId { get; set; }
        public string SubTopicName { get; set; } = string.Empty;
        public Guid TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
