using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class SubTopicConfiguration : IEntityTypeConfiguration<SubTopic>
    {
        public void Configure(EntityTypeBuilder<SubTopic> builder)
        {
            builder.HasKey(st => st.SubTopicId);

            builder.Property(st => st.SubTopicName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(st => st.Topic)
                .WithMany(t => t.SubTopics)
                .HasForeignKey(st => st.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
