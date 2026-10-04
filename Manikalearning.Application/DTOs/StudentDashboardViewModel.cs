namespace Manikalearning.Application.DTOs
{
    public class StudentDashboardViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string TargetExam { get; set; } = "UGC NET Paper 1";
        public int ReadinessScore { get; set; }
        public int TotalQuestionsSolved { get; set; }
        public double AccuracyPercentage { get; set; }
        public int? AllIndiaRank { get; set; }
        public int ActiveStreakDays { get; set; }
        public int DailySolvedMcqs { get; set; }
        public int DailyGoalMcqs { get; set; } = 50;

        // Quick Counts
        public int ActiveMockTestsCount { get; set; }
        public int MistakeBookCount { get; set; }
        public int TotalPyqSolvedCount { get; set; }
        public int BookmarksCount { get; set; }
         
        public List<CourseProgressDto> EnrolledCourses { get; set; } = new();
        public List<SubjectStrengthDto> SubjectStrengths { get; set; } = new();
        public List<UpcomingTestDto> UpcomingMockTests { get; set; } = new();
    }

    public class CourseProgressDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CurrentModule { get; set; } = string.Empty;
        public int ProgressPercentage { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int EstimatedMinutesLeft { get; set; }
    }

    public class SubjectStrengthDto
    {
        public string SubjectName { get; set; } = string.Empty;
        public double AccuracyPercentage { get; set; }
        public string StatusLevel => AccuracyPercentage >= 75 ? "Strong" : (AccuracyPercentage >= 50 ? "Moderate" : "Needs Focus");
        public string BadgeClass => AccuracyPercentage >= 75 ? "bg-success" : (AccuracyPercentage >= 50 ? "bg-primary" : "bg-warning");
    }

    public class UpcomingTestDto
    {
        public int TestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsLive { get; set; }
    }
}
