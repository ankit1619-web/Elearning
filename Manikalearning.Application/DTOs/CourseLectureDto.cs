namespace Manikalearning.Application.DTOs
{
    public class CourseLectureDto
    {
        public Guid LectureId { get; set; }
        public Guid CourseSectionId { get; set; }
        public string LectureTitle { get; set; }
        public string Description { get; set; }
        public string VideoUrl { get; set; }
        public int? VideoDurationSeconds { get; set; }
        public string NotesUrl { get; set; }
        public bool IsFreePreview { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
