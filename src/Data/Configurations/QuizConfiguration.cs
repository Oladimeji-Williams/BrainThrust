using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasKey(q => q.Id);
            builder.Property(q => q.Title).IsRequired().HasMaxLength(200);

            builder.HasOne(q => q.Topic)
                .WithOne(m => m.Quiz)  // ✅ One-to-One relationship
                .HasForeignKey<Quiz>(q => q.TopicId) // ✅ Foreign Key in Quiz
                .OnDelete(DeleteBehavior.Cascade); // ✅ If Topic is deleted, delete Quiz
        }
    }
}
