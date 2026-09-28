namespace Manikalearning.Application.DTOs
{
    public class BookmarkDto
    {
        public Guid BookmarkId { get; set; }
        public Guid UserId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;

        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string SubjectOrCourse { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    public class BookmarkPagedResultDto
    {
        public List<BookmarkDto> Bookmarks { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
