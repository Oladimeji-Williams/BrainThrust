using BrainThrust.src.Dtos.ReportDtos;

namespace BrainThrust.src.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalUsers();
        Task<int> GetActiveUsers();
        Task<int> GetNewUsers();
        Task<int> GetTotalSubjects();
        Task<List<MostEnrolledSubjectDto>> GetMostEnrolledSubjects();
        Task<List<TopScorerDto>> GetTopScorers();
        Task<List<(int UserId, int QuizId, int TotalScore, bool IsPassed, DateTime Created)>> GetRecentQuizAttempts();
    }
}
