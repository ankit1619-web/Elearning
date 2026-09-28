using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manikalearning.Domain.Entities
{
    [Table("Users")]
    public class User : IdentityUser<Guid>
    {
        public User()
        {
            Id = Guid.NewGuid();
        }

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Mobile { get; set; }

        public string? ProfileImage { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsMobileVerified { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
        public ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
        public ICollection<QuestionMistake> QuestionMistakes { get; set; } = new List<QuestionMistake>();
        public ICollection<StudentSubscription> StudentSubscriptions { get; set; } = new List<StudentSubscription>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
    [Table("Roles")]
    public class Role : IdentityRole<Guid>
    {
        public Role() : base() { }

        public Role(string roleName) : base(roleName) { }

        // Navigation Property
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
    

    [Table("UserRoles")]
    public class UserRole : IdentityUserRole<Guid>
    {
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;
    }
}