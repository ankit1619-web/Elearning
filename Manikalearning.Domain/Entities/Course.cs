using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Courses")]
    public class Course 
    {
        [Key]
        public Guid CourseId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string CourseName { get; set; } 
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsPublished { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<PlanCourse> PlanCourses { get; set; } 
        public virtual ICollection<CourseSection> CourseSections { get; set; } = new List<CourseSection>(); 
    }

    [Table("CourseSections")]
    public class CourseSection
    {
        [Key]
        public Guid CourseSectionId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(250)]
        public string SectionName { get; set; }

        public string Description { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        public virtual ICollection<CourseLecture> CourseLectures { get; set; } = new List<CourseLecture>();
    }
    [Table("CourseLectures")]
    public class CourseLecture
    {
        [Key]
        public Guid LectureId { get; set; }

        [Required]
        public Guid CourseSectionId { get; set; }

        [Required]
        [StringLength(250)]
        public string LectureTitle { get; set; }

        public string? Description { get; set; }

        [StringLength(1000)]
        public string VideoUrl { get; set; }

        public int? VideoDurationSeconds { get; set; }

        [StringLength(1000)]
        public string NotesUrl { get; set; }

        [Column("Notes")]
        public string? Notes { get; set; }
        [Required]
        public bool IsFreePreview { get; set; }

        [Required]
        public int DisplayOrder { get; set; }

        [Required]
        public bool IsPublished { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CourseSectionId")]
        public virtual CourseSection CourseSection { get; set; }
        public virtual ICollection<StudentLectureProgress> StudentLectureProgresses { get; set; } = new List<StudentLectureProgress>();
    }

    [Table("StudentLectureProgress")]
    public class StudentLectureProgress
    {
        [Key]
        public Guid ProgressId { get; set; }

        public Guid UserId { get; set; }

        public Guid LectureId { get; set; }

        public int WatchedSeconds { get; set; }

        public int TotalSeconds { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(LectureId))]
        public virtual CourseLecture Lecture { get; set; } = null!;
    }

    [Table("StudentCourses")]
    public class StudentCourse
    {
        [Key]
        public Guid StudentCourseId { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
         
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; } = null!;
    }
}
