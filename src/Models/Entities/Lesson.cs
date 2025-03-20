using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Lesson : BaseEntity
    {
        [Required]
        public required int TopicId { get; set; }

        [Required]
        public required string Title { get; set; }

        public string? Content { get; set; }
        [Required]
        public required string VideoUrl { get; set; }

        [ForeignKey(nameof(TopicId))]
        public Topic? Topic { get; set; }
        
    }
}