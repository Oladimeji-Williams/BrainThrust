using BrainThrust.src.Dtos.ReportDtos;
using BrainThrust.src.Mappers;
using BrainThrust.src.Repositories.Interfaces;
using BrainThrust.src.Services.Interfaces;
using System.Security.Claims;

namespace BrainThrust.src.Services.Classes
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IUserService _userService;

        public DashboardService(IDashboardRepository dashboardRepository, IUserService userService)
        {
            _dashboardRepository = dashboardRepository;
            _userService = userService;
        }

        public async Task<DashboardDto> GetDashboardData(ClaimsPrincipal user)
        {
            var userId = _userService.GetLoggedInUserId(user);
            if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

            var totalUsers = await _dashboardRepository.GetTotalUsers();
            var activeUsers = await _dashboardRepository.GetActiveUsers();
            var newUsers = await _dashboardRepository.GetNewUsers();
            var totalSubjects = await _dashboardRepository.GetTotalSubjects();
            var totalTopics = await _dashboardRepository.GetTotalTopics();
            var totalLessons = await _dashboardRepository.GetTotalLessons();
            var totalQuizzes = await _dashboardRepository.GetTotalQuizzes();
            var mostEnrolledSubjects = await _dashboardRepository.GetMostEnrolledSubjects();
            var recentQuizAttempts = await _dashboardRepository.GetRecentQuizAttempts();
            var topScorers = await _dashboardRepository.GetTopScorers();

            // ✅ Fetch completed items for the authenticated user
            var completedSubjects = await _dashboardRepository.GetCompletedSubjects(userId.Value);
            var completedTopics = await _dashboardRepository.GetCompletedTopics(userId.Value);
            var completedLessons = await _dashboardRepository.GetCompletedLessons(userId.Value);
            var completedQuizzes = await _dashboardRepository.GetCompletedQuizzes(userId.Value);

            return DashboardMapper.ToDashboardDto(
                totalUsers, activeUsers, newUsers,
                totalSubjects, totalTopics, totalLessons, totalQuizzes,  
                completedSubjects, completedTopics, completedLessons, completedQuizzes,  // ✅ Updated values
                0,  // Placeholder for active learners (Update if needed)
                mostEnrolledSubjects?.Select(s => (s.SubjectId, s.Enrollments)).ToList() ?? new(),
                recentQuizAttempts ?? new(),
                topScorers?.Select(t => (t.UserId, t.TotalScore)).ToList() ?? new List<(int, double)>()
            );
        }
    }
}
