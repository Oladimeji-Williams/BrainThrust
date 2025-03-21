using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models.Dtos
{
    public class QuizDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int TopicId { get; set; }

        [Required]
        public ICollection<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
