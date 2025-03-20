using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class TopicProgress : BaseEntity
    {
        [Required]
        public required int TopicId { get; set; }
        [Required]
        public required int UserId { get; set; }  // Track progress per user
        public bool IsCompleted { get; set; } = false;
        public DateTime? DateCompleted { get; set; }

        // Foreign key relationship
        [ForeignKey(nameof(TopicId))]
        public Topic? Topic { get; set; } = null!;

        // ✅ Add User Navigation Property
        [ForeignKey(nameof(UserId))]
        [Required]
        public User? User { get; set; } = null!;
    }
}
