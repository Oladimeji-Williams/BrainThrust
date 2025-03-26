using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class UserQuizSubmission : BaseEntity
    {
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public int UserQuizAttemptId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        [ForeignKey("SelectedOptionId")]
        public Option? SelectedOption { get; set; }

        [ForeignKey("UserQuizAttemptId")]
        public UserQuizAttempt? UserQuizAttempt { get; set; }
    }
}
