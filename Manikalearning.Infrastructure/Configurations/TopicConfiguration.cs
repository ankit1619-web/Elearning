using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.HasKey(t => t.TopicId);

            builder.Property(t => t.TopicName)
                .IsRequired()
                .HasMaxLength(150);
            builder.HasOne(t => t.Section)
                .WithMany(sec => sec.Topics)
                .HasForeignKey(t => t.SectionId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}
