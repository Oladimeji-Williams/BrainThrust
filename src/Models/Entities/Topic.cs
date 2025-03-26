using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Topic : BaseEntity
    {
        [Required]
        public int SubjectId { get; set; }

        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }

        [ForeignKey(nameof(SubjectId))]
        [Required]
        public Subject? Subject { get; set; }
        [Required]

        public ICollection<Lesson>? Lessons { get; set; } = new List<Lesson>();
        [Required]

        public Quiz? Quiz { get; set; }
    }
}



