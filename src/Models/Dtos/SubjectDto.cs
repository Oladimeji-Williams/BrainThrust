namespace BrainThrust.src.Models.Dtos
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public List<TopicDto> Topics { get; set; } = new();
    }
}
