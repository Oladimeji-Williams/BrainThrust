using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models.Entities
{
    public class User: BaseEntity
    {

        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required byte[] PasswordHash { get; set; }

        [Required]
        public required byte[] PasswordSalt { get; set; }

        public string Role { get; set; } = "User";

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public string? PasswordResetToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        // Define relationship with Enrollment
        public List<Enrollment>? Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<UserQuizSubmission>? QuizSubmissions { get; set; }

    }
}