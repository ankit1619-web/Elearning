using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class DoubtConfiguration : IEntityTypeConfiguration<DoubtEntity>
    {
        public void Configure(EntityTypeBuilder<DoubtEntity> builder)
        {
            builder.ToTable("Doubts");

            builder.HasKey(d => d.DoubtId);

            builder.Property(d => d.QuestionText)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(d => d.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.Property(d => d.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.IsActive)
                .HasDefaultValue(true);

            builder.Property(d => d.IsDeleted)
                .HasDefaultValue(false);
             
            builder.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(d => d.Course)
                .WithMany()
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(d => d.Lecture)
                .WithMany()
                .HasForeignKey(d => d.LectureId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
