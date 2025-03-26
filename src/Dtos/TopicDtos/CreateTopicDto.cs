using System.ComponentModel.DataAnnotations;
namespace BrainThrust.src.Dtos.TopicDtos
{
    public class CreateTopicDto
    {
        [Required]
        public string? Title { get; set; }

        public string? Description { get; set; }
    }

}
