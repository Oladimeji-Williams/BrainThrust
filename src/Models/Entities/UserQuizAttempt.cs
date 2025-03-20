using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class UserQuizAttempt : BaseEntity
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int QuizId { get; set; }
        [Required]
        public int CorrectAnswers { get; set; }
        [Required]
        public int IncorrectAnswers { get; set; }
        [Required]
        public int TotalQuestions { get; set; }
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public double TotalScore { get; set; }
        [Required]
        public bool IsPassed { get; set; } // Passed if TotalScore >= 60

        // ✅ Ensure User navigation
        [ForeignKey("UserId")]
        public User User { get; set; }

        // ✅ Ensure Quiz navigation
        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        // ✅ Ensure proper navigation for submissions
        public virtual ICollection<UserQuizSubmission> Submissions { get; set; } = new List<UserQuizSubmission>();
    }
}
