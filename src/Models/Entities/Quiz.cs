using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Quiz : BaseEntity
    {
        [Required]
        public int TopicId { get; set; }
        
        [Required]
        public string? Title { get; set; }
        
        [ForeignKey(nameof(TopicId))]
        public Topic? Topic { get; set; }
        public ICollection<Question>? Questions { get; set; } = new List<Question>();
    }
}
