using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models.Entities
{
    public class Subject : BaseEntity
    {
        [Required]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }

        // Define relationship with Enrollment
        public ICollection<Enrollment>? Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Topic>? Topics { get; set; } = new List<Topic>();
    }
}
