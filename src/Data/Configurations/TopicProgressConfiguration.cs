using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class TopicProgressConfiguration : IEntityTypeConfiguration<TopicProgress>
    {
        public void Configure(EntityTypeBuilder<TopicProgress> builder)
        {
            builder.HasKey(mp => mp.Id);
            builder.Property(mp => mp.IsCompleted).IsRequired();
            builder.Property(mp => mp.DateCompleted).IsRequired(false);

            builder.HasOne(mp => mp.Topic)
                .WithMany()
                .HasForeignKey(mp => mp.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(mp => mp.User)
                .WithMany()
                .HasForeignKey(mp => mp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
