using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class QuestionOptionConfiguration
    : IEntityTypeConfiguration<QuestionOption>
    { 
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.ToTable("QuestionOptions");

            builder.HasKey(x => x.OptionId);

            builder.Property(x => x.OptionLabel)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.OptionText)
                .IsRequired();

            builder.HasIndex(x => new
            {
                x.QuestionId,
                x.DisplayOrder
            });
        }
    }
}
