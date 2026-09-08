using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class PaperConfiguration : IEntityTypeConfiguration<Paper>
    {
        public void Configure(EntityTypeBuilder<Paper> builder)
        {
            builder.ToTable("Papers");

            builder.HasKey(x => x.PaperId);

            builder.Property(x => x.PaperId)
                .ValueGeneratedNever();

            builder.Property(x => x.PaperName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.ExamId)
                .IsRequired();

            builder.HasOne(x => x.Exam)
                .WithMany(x => x.Papers)
                .HasForeignKey(x => x.ExamId)
                .HasPrincipalKey(x => x.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Questions)
                .WithOne(x => x.Paper)
                .HasForeignKey(x => x.PaperId)
                .HasPrincipalKey(x => x.PaperId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
