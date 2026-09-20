using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Manikalearning.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
             
            builder.Property(u => u.FullName).HasMaxLength(150).IsRequired();
            builder.Property(u => u.Mobile).HasMaxLength(20);
            builder.Property(u => u.ProfileImage).HasMaxLength(500); 
        }
    }
}
