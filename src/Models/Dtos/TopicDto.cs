using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models.Dtos
{
    public class TopicDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public string Description { get; set; }
    }

}
