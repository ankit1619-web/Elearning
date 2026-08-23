namespace Manikalearning.Web.Areas.Student.ViewModels
{
    public class DashboardVM
    {
        public string StudentName { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }

        public int TotalTests { get; set; }

        public int CompletedTests { get; set; }

        public int TotalPYQAttempted { get; set; }

        public int TotalQuestionsAttempted { get; set; }

        public decimal AverageScore { get; set; }

        public decimal BestScore { get; set; }

        public int CurrentStreak { get; set; }

        public List<RecentTestVM> RecentTests { get; set; } = new();

        public List<UpcomingTestVM> UpcomingTests { get; set; } = new();

        public List<ContinueCourseVM> ContinueCourses { get; set; } = new();
    }

    public class RecentTestVM
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public decimal ObtainedMarks { get; set; }

        public decimal TotalMarks { get; set; }

        public DateTime? SubmittedAt { get; set; }
    }

    public class UpcomingTestVM
    {
        public Guid TestId { get; set; }

        public string TestName { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public DateTime? StartDate { get; set; }
    }

    public class ContinueCourseVM
    {
        public Guid CourseId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public int ProgressPercentage { get; set; }

        public Guid? LastLectureId { get; set; }
    }
}
