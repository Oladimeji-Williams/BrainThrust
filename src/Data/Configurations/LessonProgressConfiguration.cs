using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
    {
        public void Configure(EntityTypeBuilder<LessonProgress> builder)
        {
            builder.HasKey(lp => lp.Id);
            builder.Property(lp => lp.IsCompleted).IsRequired();
            builder.Property(lp => lp.DateCompleted).IsRequired(false);

            builder.HasOne(lp => lp.Lesson)
                .WithMany()
                .HasForeignKey(lp => lp.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(lp => lp.User)
                .WithMany()
                .HasForeignKey(lp => lp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
