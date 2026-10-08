using Manikalearning.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Manikalearning.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<
        User,
        Role,
        Guid,
        IdentityUserClaim<Guid>,
        UserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Domain Sets
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Paper> Papers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<SubTopic> SubTopics { get; set; }

        public DbSet<ExamCourse> ExamCourses { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseSection> CourseSections { get; set; }
        public DbSet<CourseLecture> CourseLectures { get; set; }
        public DbSet<StudentLectureProgress> StudentLectureProgresses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<PlanCourse> PlanCourses { get; set; }
        public DbSet<StudentSubscription> StudentSubscriptions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<QuestionPassage> QuestionPassages => Set<QuestionPassage>();
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }

        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<TestAttemptAnswer> TestAttemptAnswers { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<QuestionMistake> QuestionMistakes { get; set; }
        public DbSet<TopicContent> TopicContents { get; set; }
        public DbSet<DoubtEntity> Doubts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
             
            modelBuilder.Entity<User>(entity => entity.ToTable("Users"));
            modelBuilder.Entity<Role>(entity => entity.ToTable("Roles"));
            modelBuilder.Entity<UserRole>(entity => entity.ToTable("UserRoles"));
            modelBuilder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UserClaims"));
            modelBuilder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UserLogins"));
            modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RoleClaims"));
            modelBuilder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens"));
             
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
             
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

                entity.HasKey(x => new { x.PlanId, x.CourseId });

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

                entity.HasKey(x => new { x.ExamId, x.CourseId });
            });

            modelBuilder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            }); 
            modelBuilder.Entity<TopicContent>()
                 .HasKey(tc => tc.ContentId);  
            modelBuilder.Entity<TopicContent>()
                .HasOne(tc => tc.Topic)
                .WithOne()
                .HasForeignKey<TopicContent>(tc => tc.TopicId);
        }
    }
}