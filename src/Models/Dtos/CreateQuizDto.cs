namespace BrainThrust.src.Models.Dtos
{
    public class CreateQuizDto
    {
        public string Title { get; set; } = string.Empty;
        public int TopicId { get; set; }
    }
}
