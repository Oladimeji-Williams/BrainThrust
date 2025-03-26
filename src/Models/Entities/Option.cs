using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Option : BaseEntity
    {
        [Required]
        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        [Required]
        public string? Text { get; set; }

        public Question? Question { get; set; }
    }
}
