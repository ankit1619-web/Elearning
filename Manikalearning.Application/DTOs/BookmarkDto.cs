namespace Manikalearning.Application.DTOs
{
    public class BookmarkDto
    {
        public Guid BookmarkId { get; set; }
        public Guid UserId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;

        public Guid? QuestionId { get; set; }
        public Guid? LectureId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string SubjectOrCourse { get; set; } = string.Empty;  
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public DateTime SavedAt { get; set; }
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
     
}
