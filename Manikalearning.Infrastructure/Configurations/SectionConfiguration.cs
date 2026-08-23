using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class SectionConfiguration : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(sec => sec.SectionId);

            builder.Property(sec => sec.SectionName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(sec => sec.Subject)
                .WithMany(s => s.Sections)
                .HasForeignKey(sec => sec.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
