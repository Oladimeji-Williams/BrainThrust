using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Data.Configurations
{
    public class UserQuizSubmissionConfiguration : IEntityTypeConfiguration<UserQuizSubmission>
    {
        public void Configure(EntityTypeBuilder<UserQuizSubmission> builder)
        {
            builder.HasKey(uqa => uqa.Id);

            // ✅ Define relationship with User
            builder.HasOne(uqa => uqa.User)
                .WithMany() // Assuming no collection property in User
                .HasForeignKey(uqa => uqa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

 
            // ✅ Define relationship with Quiz
            builder.HasOne(uqa => uqa.Quiz)
                .WithMany()
                .HasForeignKey(uqa => uqa.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
