using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
         
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; } 

        public ICollection<UserRole> UserRoles { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
         
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
         
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<TestAttemptAnswer> TestAttemptAnswers { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<QuestionMistake> QuestionMistakes { get; set; }
         
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanCourse> PlanCourses { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
            modelBuilder.Entity<PlanCourse>()
                .HasKey(pc => new { pc.PlanId, pc.CourseId });
             
            modelBuilder.Entity<Bookmark>()
                .HasIndex(b => new { b.UserId, b.QuestionId })
                .IsUnique();

            modelBuilder.Entity<QuestionMistake>()
                .HasIndex(qm => new { qm.UserId, qm.QuestionId })
                .IsUnique();

            modelBuilder.Entity<Coupon>()
                .HasIndex(c => c.CouponCode)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<UserRole>()
             .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<TestAttemptAnswer>()
                .HasOne(taa => taa.SelectedOption)
                .WithMany(qo => qo.TestAttemptAnswers)
                .HasForeignKey(taa => taa.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);
             
            modelBuilder.Entity<StudentSubscription>()
                .HasOne(ss => ss.Payment)
                .WithMany(p => p.StudentSubscriptions)
                .HasForeignKey(ss => ss.PaymentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
