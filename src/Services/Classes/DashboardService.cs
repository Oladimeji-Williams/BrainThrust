using BrainThrust.src.Dtos.ReportDtos;
using BrainThrust.src.Repositories.Interfaces;
using BrainThrust.src.Services.Interfaces;

namespace BrainThrust.src.Services.Classes
{

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardDto> GetDashboardData()
        {
            var totalUsers = await _dashboardRepository.GetTotalUsers();
            var activeUsers = await _dashboardRepository.GetActiveUsers();
            var newUsers = await _dashboardRepository.GetNewUsers();
            var totalSubjects = await _dashboardRepository.GetTotalSubjects();
            var mostEnrolledSubjects = await _dashboardRepository.GetMostEnrolledSubjects();
            var recentQuizAttempts = await _dashboardRepository.GetRecentQuizAttempts();
            var topScorers = await _dashboardRepository.GetTopScorers();



            return new DashboardDto
            {
                Users = new UserStatsDto
                {
                    TotalUsers = totalUsers,
                    ActiveUsers = activeUsers,
                    NewUsers = newUsers
                },
                Content = new ContentStatsDto
                {
                    TotalSubjects = totalSubjects,
                    MostEnrolledSubjects = mostEnrolledSubjects?
                        .Select(s => (s.SubjectId, s.Enrollments))
                        .ToList() ?? new List<(int, int)>()
                },
                Engagement = new EngagementStatsDto
                {
                    TopScorers = topScorers
                        .Select(t => (t.UserId, t.TotalScore))
                        .ToList() ?? new List<(int, double)>()

                },
                QuizPerformance = new QuizPerformanceDto
                {
                    RecentQuizAttempts = recentQuizAttempts.Select(q => new RecentQuizAttemptDto
                    {
                        UserId = q.UserId,
                        QuizId = q.QuizId,
                        TotalScore = q.TotalScore,
                        IsPassed = q.IsPassed,
                        Created = q.Created
                    }).ToList()
                }
            };

        }
    }
}
