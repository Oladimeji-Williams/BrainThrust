namespace BrainThrust.src.Models.Dtos
{
    public class GetTopicDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public List<int> LessonIds { get; set; } = new List<int>();
    }
}
