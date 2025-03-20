namespace BrainThrust.src.Models.Dtos
{
    public class UpdateQuizDto
    {
        public string? Title { get; set; }
        public int? TopicId { get; set; }
        public bool? IsDeleted { get; set; }
    }

}
