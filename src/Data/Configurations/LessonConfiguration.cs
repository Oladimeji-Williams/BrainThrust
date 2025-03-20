using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Title).IsRequired().HasMaxLength(200);
            builder.HasOne(l => l.Topic)
                .WithMany(m => m.Lessons)
                .HasForeignKey(l => l.TopicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
