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
        public DbSet<UserRole> UserRoles { get; set; }
         
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Paper> Papers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<SubTopic> SubTopics { get; set; }
         
        public DbSet<ExamCourse> ExamCourse { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSection> CourseSection { get; set; }
        public DbSet<CourseLecture> CourseLecture { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanCourse> PlanCourses { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
         
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
         
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<TestAttemptAnswer> TestAttemptAnswers { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<QuestionMistake> QuestionMistakes { get; set; } 
        public DbSet<QuestionPassage> QuestionPassages  => Set<QuestionPassage>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
             
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
            modelBuilder.Entity<PlanCourse>(entity =>
                {
                    entity.ToTable("PlanCourses");

                    entity.HasKey(x => new
                    {
                        x.PlanId,
                        x.CourseId
                    });

                    entity.HasOne(x => x.Plan)
                        .WithMany(x => x.PlanCourses)
                        .HasForeignKey(x => x.PlanId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(x => x.Course)
                        .WithMany(x => x.PlanCourses)
                        .HasForeignKey(x => x.CourseId)
                        .OnDelete(DeleteBehavior.Cascade);
                });
            modelBuilder.Entity<ExamCourse>(entity =>
            {
                entity.ToTable("ExamCourses");

                entity.HasKey(x => new
                {
                    x.ExamId,
                    x.CourseId
                }); 
            });
        }
    }
}
