namespace Manikalearning.Application.DTOs
{
    public class StudentCourseDto
    {
        public Guid StudentCourseId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;

        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string? CategoryName { get; set; }

        public DateTime EnrolledAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    }

    public class StudentCoursePagedResultDto
    {
        public List<StudentCourseDto> Enrollments { get; set; } = new();
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int InactiveCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class UserSelectOptionDto
    {
        public Guid UserId { get; set; }
        public string DisplayText { get; set; } = string.Empty;
    }

    public class CourseSelectOptionDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
