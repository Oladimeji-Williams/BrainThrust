namespace BrainThrust.src.Dtos.ReportDtos
{
    public class QuizPerformanceDto
    {
        public List<RecentQuizAttemptDto>? RecentQuizAttempts { get; set; }
        public List<TopScorerDto>? TopScorers { get; set; }
        public int TotalQuizAttempts { get; set; } // ✅ NEW
        public int UniqueQuizzesAttempted { get; set; } // ✅ NEW
    }
}