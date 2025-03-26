using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class LessonProgress : BaseEntity
    {
        [Required]
        public required int LessonId { get; set; }
        [Required]
        public required int UserId { get; set; }   // Track progress per user
        public bool IsCompleted { get; set; } = false;
        public DateTime? DateCompleted { get; set; }

        // Foreign key relationship
        [ForeignKey(nameof(LessonId))]
        [Required]
        public Lesson? Lesson { get; set; }

        // ✅ Add User Navigation Property
        [ForeignKey(nameof(UserId))]
        [Required]
        public User? User { get; set; }
    }
}


