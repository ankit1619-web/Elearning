using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class EnrolledCourseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int TotalModules { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime EnrolledDate { get; set; }
        public DateTime? LastAccessedDate { get; set; }
        public int? NextLessonId { get; set; }
        public string? NextLessonTitle { get; set; }
        public bool IsCompleted { get; set; }
        public string? CertificateUrl { get; set; }
    }

    public class MyCoursesViewModel
    {
        public List<EnrolledCourseDto> Courses { get; set; } = new();
        public int TotalEnrolled { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public string SelectedFilter { get; set; } = "All";
        public string SearchQuery { get; set; } = string.Empty;
    }
}
