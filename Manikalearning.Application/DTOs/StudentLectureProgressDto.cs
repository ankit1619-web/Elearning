namespace Manikalearning.Application.DTOs
{
    public class StudentLectureProgressDto
    {
        public Guid ProgressId { get; set; }
        public Guid UserId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;

        public Guid LectureId { get; set; }
        public string LectureTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;

        public int WatchedSeconds { get; set; }
        public int TotalSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime LastWatchedAt { get; set; }

        // Computed Properties
        public decimal CompletionPercentage => TotalSeconds > 0
            ? Math.Min(100, Math.Round((decimal)WatchedSeconds * 100 / TotalSeconds, 2))
            : 0;

        public string FormattedWatchedTime => TimeSpan.FromSeconds(WatchedSeconds).ToString(@"hh\:mm\:ss");
        public string FormattedTotalTime => TimeSpan.FromSeconds(TotalSeconds).ToString(@"hh\:mm\:ss");
    }

    public class ProgressPagedResultDto
    {
        public List<StudentLectureProgressDto> ProgressRecords { get; set; } = new();
        public int TotalCount { get; set; }
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
        public decimal AverageCompletionRate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
