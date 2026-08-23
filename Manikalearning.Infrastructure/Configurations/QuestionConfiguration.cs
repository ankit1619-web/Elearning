using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class QuestionConfiguration
    : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(x => x.QuestionId);

            builder.Property(x => x.QuestionText)
                .IsRequired();

            builder.Property(x => x.QuestionType)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Difficulty)
                .HasConversion<int>();

            builder.Property(x => x.Explanation);

            builder.Property(x => x.ShortExplanation);

            builder.Property(x => x.Marks)
                .HasPrecision(10, 2);

            builder.Property(x => x.NegativeMarks)
                .HasPrecision(10, 2);

            builder.Property(x => x.PYQSession)
                .HasMaxLength(100);

            builder.Property(x => x.SourceName)
                .HasMaxLength(250);

            builder.HasIndex(x => x.ExamId);

            builder.HasIndex(x => x.PaperId);

            builder.HasIndex(x => x.SubjectId);

            builder.HasIndex(x => x.TopicId);

            builder.HasIndex(x => x.IsPYQ);

            builder.HasIndex(x => new
            {
                x.IsPYQ,
                x.PYQYear
            });

            // Exam
            builder.HasOne(x => x.Exam)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paper
            builder.HasOne(x => x.Paper)
                .WithMany()
                .HasForeignKey(x => x.PaperId)
                .OnDelete(DeleteBehavior.Restrict);

            // Subject
            builder.HasOne(x => x.Subject)
                .WithMany()
                .HasForeignKey(x => x.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Section
            builder.HasOne(x => x.Section)
                .WithMany()
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Topic
            builder.HasOne(x => x.Topic)
                .WithMany()
                .HasForeignKey(x => x.TopicId)
                .OnDelete(DeleteBehavior.Restrict);

            // SubTopic
            builder.HasOne(x => x.SubTopic)
                .WithMany()
                .HasForeignKey(x => x.SubTopicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Creator
            builder.HasOne(x => x.Creator)
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Passage
            builder.HasOne(x => x.Passage)
                .WithMany(x => x.Questions)
                .HasForeignKey(x => x.PassageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Options
            builder.HasMany(x => x.Options)
                .WithOne(x => x.Question)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
