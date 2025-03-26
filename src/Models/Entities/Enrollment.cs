using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Enrollment : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [ForeignKey(nameof(UserId))]
        [Required]
        public User? User { get; set; }

        [ForeignKey(nameof(SubjectId))]
        [Required]
        public Subject? Subject { get; set; }
    }
}
