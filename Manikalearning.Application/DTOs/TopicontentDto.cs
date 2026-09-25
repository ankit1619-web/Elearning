namespace Manikalearning.Application.DTOs
{
    public  class TopicontentDto
    {
        public Guid Id { get; set; }
        public Guid TopicId { get; set; }
        public string TopicName { get; set; } = string.Empty; 
        public string? ContentEn { get; set; }
    }
}
