using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.HasKey(e => e.ExamId);

            builder.Property(e => e.ExamName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(e => e.ExamCode)
                .IsUnique();

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
