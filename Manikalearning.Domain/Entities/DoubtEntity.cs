namespace Manikalearning.Domain.Entities
{ 
    public class DoubtEntity
    {
        public Guid DoubtId { get; set; }
        public Guid UserId { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? LectureId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // "Pending", "Resolved", "InReview"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
         
        public virtual User User { get; set; } = null!;
        public virtual Course? Course { get; set; }
        public virtual CourseLecture? Lecture { get; set; }
    }
}
