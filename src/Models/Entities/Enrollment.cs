using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Enrollment : BaseEntity
    {
        [Required]
        public required int UserId { get; set; }  // Foreign key to User

        [Required]
        public required int SubjectId { get; set; } // Foreign key to Course

        [ForeignKey(nameof(UserId))]
        [Required]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(SubjectId))]
        [Required]
        public Subject Subject { get; set; } = null!;
    }
}
