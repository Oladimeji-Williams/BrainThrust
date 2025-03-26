namespace BrainThrust.src.Dtos.TopicDtos
{
    public class GetTopicDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public List<int> LessonIds { get; set; } = new List<int>();
    }
}
