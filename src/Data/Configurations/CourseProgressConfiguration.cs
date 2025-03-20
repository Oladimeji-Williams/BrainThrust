using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class SubjectProgressConfiguration : IEntityTypeConfiguration<SubjectProgress>
    {
        public void Configure(EntityTypeBuilder<SubjectProgress> builder)
        {
            builder.HasKey(cp => cp.Id);
            builder.Property(cp => cp.IsCompleted).IsRequired();
            builder.Property(cp => cp.DateCompleted).IsRequired(false);

            builder.HasOne(cp => cp.Subject)
                .WithMany()
                .HasForeignKey(cp => cp.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.User)
                .WithMany()
                .HasForeignKey(cp => cp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
