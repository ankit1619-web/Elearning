namespace Manikalearning.Application.DTOs
{
    public class CourseLectureDto
    {
        public Guid CourseLectureId { get; set; }

        public Guid CourseSectionId { get; set; }

        public string LectureTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }

        public int DurationMinutes { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsFreePreview { get; set; }

        public bool IsActive { get; set; }
    }
}
