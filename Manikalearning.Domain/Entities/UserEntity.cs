using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Users")]
    public class User 
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Mobile { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string ProfileImage { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public bool IsMobileVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
         
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<TestAttempt> TestAttempts { get; set; }
        public ICollection<Bookmark> Bookmarks { get; set; }
        public ICollection<QuestionMistake> QuestionMistakes { get; set; }
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; }
        public ICollection<Payment> Payments { get; set; } 
    }

    [Table("Roles")]
    public class Role
    {
        [Key]
        public Guid RoleId { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string RoleName { get; set; }  
          
        public ICollection<UserRole> UserRoles { get; set; }
    }

    [Table("UserRoles")]
    public class UserRole
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid RoleId { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
    }
}
