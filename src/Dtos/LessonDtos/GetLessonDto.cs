namespace BrainThrust.src.Dtos.LessonDtos
{
    public class GetLessonDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int TopicId { get; set; }
    }

}
