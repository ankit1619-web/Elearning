namespace Manikalearning.Application.DTOs
{
    public class CourseSectionDto
    {
        public Guid CourseSectionId { get; set; }
        public Guid CourseId { get; set; }
        public string SectionName { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public List<CourseLectureDto> Lectures { get; set; } = new();
    }
}
