using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Title).IsRequired().HasMaxLength(200);
            builder.HasOne(m => m.Subject)
                .WithMany(c => c.Topics)
                .HasForeignKey(m => m.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
