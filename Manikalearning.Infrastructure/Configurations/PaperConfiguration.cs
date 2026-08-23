using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class PaperConfiguration : IEntityTypeConfiguration<Paper>
    {
        public void Configure(EntityTypeBuilder<Paper> builder)
        {
            builder.HasKey(p => p.PaperId);

            builder.Property(p => p.PaperName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(p => p.Exam)
                .WithMany(e => e.Papers)
                .HasForeignKey(p => p.ExamId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
