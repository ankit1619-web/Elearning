namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class CourseVM
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public bool IsPurchased { get; set; }

        public int ProgressPercentage { get; set; }


        public List<CourseSectionVM> Sections { get; set; } = new();


        // List
        public List<CourseVM> Items { get; set; } = new();

        public string? Search { get; set; }
    }


    public class CourseSectionVM
    {
        public Guid CourseSectionId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public List<CourseLectureVM> Lectures { get; set; } = new();
    }


    public class CourseLectureVM
    {
        public Guid CourseLectureId { get; set; }

        public string LectureTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }

        public int DurationMinutes { get; set; }

        public bool IsFreePreview { get; set; }

        public bool IsCompleted { get; set; }
    }
}
