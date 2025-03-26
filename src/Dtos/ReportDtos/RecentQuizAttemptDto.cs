namespace BrainThrust.src.Dtos.ReportDtos
{
    public class RecentQuizAttemptDto
    {
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public double TotalScore { get; set; }
        public bool IsPassed { get; set; }
        public DateTime Created { get; set; }
    }
}