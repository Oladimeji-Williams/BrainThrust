using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models.Entities
{
    public class User: BaseEntity
    {

        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public byte[]? PasswordHash { get; set; }

        [Required]
        public byte[]? PasswordSalt { get; set; }

        public string Role { get; set; } = "User";

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public List<Enrollment>? Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<UserQuizSubmission>? QuizSubmissions { get; set; }

    }
}