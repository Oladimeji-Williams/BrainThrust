namespace BrainThrust.src.Models.Dtos
{
    public class UserProgressDto
    {
        public int UserId { get; set; }
        public int? SubjectId { get; set; }
        public int? LessonId { get; set; }
        public int? TopicId { get; set; } // Changed to nullable
        public bool IsCompleted { get; set; }
    }

}
