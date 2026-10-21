using System.ComponentModel.DataAnnotations;

namespace Manikalearning.Application.DTOs
{
    public class EnrolledCourseDto
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int TotalModules { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int ProgressPercentage { get; set; }
        public DateTime EnrolledDate { get; set; }
        public DateTime? LastAccessedDate { get; set; }
        public Guid? NextLessonId { get; set; }
        public string? NextLessonTitle { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFree { get; set; }
    }

    public class MyCoursesViewModel
    {
        public List<EnrolledCourseDto> Courses { get; set; } = new();
        public int TotalEnrolled { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public string ExamName { get; set; } = "";
        public string SelectedFilter { get; set; } = "All";
        public string SearchQuery { get; set; } = string.Empty;
    }
    public class WatchIndexViewModel
    {
        public WatchOverviewViewModel CourseOverview { get; set; } = new();
        public LessonPlayerViewModel CurrentLesson { get; set; } = new();
    }
    public class WatchOverviewViewModel
    {
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int TotalModules { get; set; }
        public int TotalLessons { get; set; }
        public int ProgressPercentage { get; set; }
        public List<ModuleDto> Modules { get; set; } = new();
    }

    public class LessonPlayerViewModel
    {
        public Guid CourseId { get; set; }
        public Guid LessonId { get; set; }
        public string LessonTitle { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string? NotesUrl { get; set; }
        public string? Notes { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsBookmarked { get; set; }
        public string ResumeSeconds { get; set; } = "0";
        public bool IsFree { get; set; } 
        public bool CanWatchVideo { get; set; } = true;
        public bool CanOpenNotes { get; set; } = true;
        public bool CanAskDoubt { get; set; } = true;
        public bool HasNotesPdf { get; set; }
        public Guid? NextLessonId { get; set; }
        public Guid? PreviousLessonId { get; set; }
        public List<ModuleDto> CourseNavigation { get; set; } = new();
    }

    public class ModuleDto
    {
        public Guid ModuleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<LessonDto> Lessons { get; set; } = new();
    }

    public class LessonDto
    {
        public Guid LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public bool IsFree { get; set; }
    }

    public class AskDoubtViewModel
    {
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Please write your question/doubt.")]
        [MinLength(10, ErrorMessage = "Doubt must be at least 10 characters long.")]
        [MaxLength(1000, ErrorMessage = "Doubt cannot exceed 1000 characters.")]
        public string DoubtText { get; set; } = string.Empty; 
    }
}
