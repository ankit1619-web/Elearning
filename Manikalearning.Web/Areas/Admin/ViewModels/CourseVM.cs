using Manikalearning.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Web.Areas.Admin.ViewModels
{
    public class CourseVM
    {
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CourseCode { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public CourseStatus Status { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;


        public List<CourseSectionVM> Sections { get; set; }
            = new();


        // List
        public string? Search { get; set; }

        public List<CourseVM> Items { get; set; } = new();

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public int TotalRecords { get; set; }
    }


    public class CourseSectionVM
    {
        public Guid CourseSectionId { get; set; }

        public Guid CourseId { get; set; }

        [Required]
        public string SectionName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public List<CourseLectureVM> Lectures { get; set; }
            = new();
    }


    public class CourseLectureVM
    {
        public Guid CourseLectureId { get; set; }

        public Guid CourseSectionId { get; set; }

        [Required]
        public string LectureTitle { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? VideoUrl { get; set; }

        public int DurationMinutes { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsFreePreview { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
