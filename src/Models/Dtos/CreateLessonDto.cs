using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models.Dtos
{
    public class CreateLessonDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string Title { get; set; }

        [Required]
        [MinLength(10)]
        public required string Content { get; set; }

        [Url]
        public required string VideoUrl { get; set; }

    }
}
