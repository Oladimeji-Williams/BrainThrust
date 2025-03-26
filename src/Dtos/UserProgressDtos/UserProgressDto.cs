namespace BrainThrust.src.Dtos.UserProgressDtos
{
    public class UserProgressDto
    {
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public int CompletedTopics { get; set; }
        public int TotalTopics { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
