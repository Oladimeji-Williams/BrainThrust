using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class SubjectProgress : BaseEntity
    {
        [Required]
        public int SubjectId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? DateCompleted { get; set; }

        [ForeignKey(nameof(SubjectId))]
        [Required]
        public Subject? Subject { get; set; }

        [ForeignKey(nameof(UserId))]
        [Required]
        public User? User { get; set; }
    }
}
