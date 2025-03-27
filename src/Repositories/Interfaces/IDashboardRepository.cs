using BrainThrust.src.Dtos.ReportDtos;

namespace BrainThrust.src.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetTotalUsers();
        Task<int> GetActiveUsers();
        Task<int> GetNewUsers();
        Task<int> GetTotalSubjects();
        Task<int> GetTotalTopics();
        Task<int> GetTotalLessons();
        Task<int> GetTotalQuizzes();
        Task<List<MostEnrolledSubjectDto>> GetMostEnrolledSubjects();
        Task<List<TopScorerDto>> GetTopScorers();
        Task<List<(int UserId, int QuizId, int TotalScore, bool IsPassed, DateTime Created)>> GetRecentQuizAttempts();
        Task<int> GetCompletedSubjects(int userId);
        Task<int> GetCompletedTopics(int userId);
        Task<int> GetCompletedLessons(int userId);
        Task<int> GetCompletedQuizzes(int userId);
        Task<int> GetUniqueQuizzesAttempted(int userId);
        Task<int> GetTotalQuizAttempts(int userId);


    }
}
