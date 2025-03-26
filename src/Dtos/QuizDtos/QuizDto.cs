using System.ComponentModel.DataAnnotations;
using BrainThrust.src.Dtos.QuestionDtos;

namespace BrainThrust.src.Dtos.QuizDtos
{
    public class QuizDto
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string? Title { get; set; }

        [Required]
        public int TopicId { get; set; }

        [Required]
        public ICollection<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}
