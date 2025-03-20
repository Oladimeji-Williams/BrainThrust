using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Quiz : BaseEntity
    {
        [Required]
        public required int TopicId { get; set; }
        
        [Required]
        public required string Title { get; set; }
        
        [ForeignKey(nameof(TopicId))]
        public Topic? Topic { get; set; }
        public ICollection<Question>? Questions { get; set; } = new List<Question>();
    }
}
