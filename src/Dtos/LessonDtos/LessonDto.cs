namespace BrainThrust.src.Dtos.LessonDtos
{
    public class LessonDto
    {
        public int LessonId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateCompleted { get; set; }
    }
}
