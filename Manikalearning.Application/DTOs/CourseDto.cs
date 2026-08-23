using Manikalearning.Domain.Enums;

namespace Manikalearning.Application.DTOs
{
    public class CourseDto
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string? CourseCode { get; set; }

        public string? Description { get; set; }

        public string? ThumbnailUrl { get; set; }

        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        public CourseStatus Status { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
