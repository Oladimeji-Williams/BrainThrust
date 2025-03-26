using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Lesson : BaseEntity
    {
        [Required]
        public int TopicId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Content { get; set; }
        [Required]
        public string? VideoUrl { get; set; }

        [ForeignKey(nameof(TopicId))]
        public Topic? Topic { get; set; }
        
    }
}