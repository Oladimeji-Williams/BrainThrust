namespace BrainThrust.src.Models.Dtos
{
    public class GetLessonDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string VideoUrl { get; set; }
        public int TopicId { get; set; }
    }

}
