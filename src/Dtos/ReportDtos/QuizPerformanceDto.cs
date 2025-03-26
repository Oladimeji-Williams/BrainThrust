namespace BrainThrust.src.Dtos.ReportDtos
{
    public class QuizPerformanceDto
    {
        public List<RecentQuizAttemptDto>? RecentQuizAttempts { get; set; }
        public List<TopScorerDto>? TopScorers { get; set; }
        
    }
}