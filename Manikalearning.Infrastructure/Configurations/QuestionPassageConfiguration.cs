using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class QuestionPassageConfiguration
    : IEntityTypeConfiguration<QuestionPassage>
    {
        public void Configure(
            EntityTypeBuilder<QuestionPassage> builder)
        {
            builder.ToTable("QuestionPassages");

            builder.HasKey(x => x.PassageId);

            builder.Property(x => x.Title)
                .HasMaxLength(250);

            builder.Property(x => x.PassageText)
                .IsRequired();
        }
    }
}
