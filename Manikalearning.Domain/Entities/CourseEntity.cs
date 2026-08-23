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

        [Required, MaxLength(200)]
        public string Slug { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsPublished { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<PlanCourse> PlanCourses { get; set; }
    }

    [Table("StudentCourses")]
    public class StudentCourse
    {
        [Key]
        public Guid StudentCourseId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required]
        public Guid CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
