using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class TopicProgress : BaseEntity
    {
        [Required]
        public int TopicId { get; set; }
        
        [Required]
        public int UserId { get; set; }

        public bool IsCompleted { get; set; } = false;
        public DateTime? DateCompleted { get; set; }

        [ForeignKey(nameof(TopicId))]
        public Topic? Topic { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
