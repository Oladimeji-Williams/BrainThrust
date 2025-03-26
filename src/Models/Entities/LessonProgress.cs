using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class LessonProgress : BaseEntity
    {
        [Required]
        public int LessonId { get; set; }
        [Required]
        public int UserId { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime? DateCompleted { get; set; }

        [ForeignKey(nameof(LessonId))]
        [Required]
        public Lesson? Lesson { get; set; }
        
        [ForeignKey(nameof(UserId))]
        [Required]
        public User? User { get; set; }
    }
}


