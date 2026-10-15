namespace Manikalearning.Application.DTOs
{
    public class BookmarkDto
    {
        public Guid BookmarkId { get; set; }
        public Guid UserId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty; 
        public string SubjectOrCourse { get; set; } = string.Empty;   
        public DateTime SavedAt { get; set; } 
        public string ItemType { get; set; } = "";  
        public Guid? QuestionId { get; set; }
        public string? QuestionText { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; } 
        public Guid? LectureId { get; set; }
        public string? LectureTitle { get; set; }
        public string? LectureDescription { get; set; }
        public int? DurationSeconds { get; set; }
        public Guid? CourseId { get; set; }
        public string? CourseName { get; set; }
    }

    public class BookmarkPagedResultDto
    {
        public List<BookmarkDto> Bookmarks { get; set; } = new();
        public int TotalCount { get; set; }
        public string? Search { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }
    public static class BookmarkType
    {
        public const string Question = "Question";
        public const string Lecture = "Lecture";
    } 

    public class BookmarkListDto
    {
        public List<BookmarkDto> Items { get; set; } = new();
        public string Type { get; set; } = BookmarkType.Question;
        public string? Search { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }                 
        public int QuestionCount { get; set; }             
        public int LectureCount { get; set; }
        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    }


}
