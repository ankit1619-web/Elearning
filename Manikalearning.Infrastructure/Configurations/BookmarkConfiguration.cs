using Manikalearning.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Infrastructure.Configurations
{
    public class BookmarkConfiguration : IEntityTypeConfiguration<Bookmark>
    {
        public void Configure(EntityTypeBuilder<Bookmark> builder)
        {
            builder.ToTable("Bookmarks");
            builder.HasKey(b => b.BookmarkId);

            builder.Property(b => b.ItemType)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookmarks) 
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(b => b.Lecture)
                .WithMany()
                .HasForeignKey(b => b.LectureId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(b => b.Question)
                .WithMany()
                .HasForeignKey(b => b.QuestionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
