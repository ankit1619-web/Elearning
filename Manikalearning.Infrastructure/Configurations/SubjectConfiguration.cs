using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.HasKey(s => s.SubjectId);

            builder.Property(s => s.SubjectName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(s => s.Paper)
                .WithMany(p => p.Subjects)
                .HasForeignKey(s => s.PaperId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
