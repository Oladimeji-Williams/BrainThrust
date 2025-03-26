namespace BrainThrust.src.Dtos.LearningProgressDtos
{
    public class LearningProgressDto
    {
        public int LessonId { get; set; }
        public string? LessonTitle { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}
