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

            
            var completedSubjects = await _dashboardRepository.GetCompletedSubjects(userId.Value);
            var completedTopics = await _dashboardRepository.GetCompletedTopics(userId.Value);
            var completedLessons = await _dashboardRepository.GetCompletedLessons(userId.Value);
            var completedQuizzes = await _dashboardRepository.GetCompletedQuizzes(userId.Value);
            var totalQuizAttempts = await _dashboardRepository.GetTotalQuizAttempts(userId.Value);
            var uniqueQuizzesAttempted = await _dashboardRepository.GetUniqueQuizzesAttempted(userId.Value);
            var activeLearners = activeUsers;

            return DashboardMapper.ToDashboardDto(
                totalUsers, activeUsers, newUsers,
                totalSubjects, totalTopics, totalLessons, totalQuizzes,  
                completedSubjects, completedTopics, completedLessons, completedQuizzes,  
                activeLearners,
                uniqueQuizzesAttempted,
                mostEnrolledSubjects?.Select(s => new MostEnrolledSubjectDto 
                { 
                    SubjectId = s.SubjectId, 
                    Enrollments = s.Enrollments 
                }).ToList() ?? new List<MostEnrolledSubjectDto>(),
                recentQuizAttempts ?? new(),
                topScorers?.Select(t => (t.UserId, t.TotalScore)).ToList() ?? new List<(int, double)>()
            );
        }



        // public async Task<DashboardDto> GetDashboardData(ClaimsPrincipal user)
        // {
        //     var userId = _userService.GetLoggedInUserId(user);
        //     if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

        //     bool isAdmin = await _userService.IsUserAdmin(user); // Assume this method exists in _userService

        //     if (isAdmin)
        //     {
        //         // Fetch full dashboard data for Admin
        //         var totalUsers = await _dashboardRepository.GetTotalUsers();
        //         var activeUsers = await _dashboardRepository.GetActiveUsers();
        //         var newUsers = await _dashboardRepository.GetNewUsers();
        //         var totalSubjects = await _dashboardRepository.GetTotalSubjects();
        //         var totalTopics = await _dashboardRepository.GetTotalTopics();
        //         var totalLessons = await _dashboardRepository.GetTotalLessons();
        //         var totalQuizzes = await _dashboardRepository.GetTotalQuizzes();
        //         var mostEnrolledSubjects = await _dashboardRepository.GetMostEnrolledSubjects();
        //         var recentQuizAttempts = await _dashboardRepository.GetRecentQuizAttempts();
        //         var topScorers = await _dashboardRepository.GetTopScorers();

        //         return DashboardMapper.ToDashboardDto(
        //             totalUsers, activeUsers, newUsers,
        //             totalSubjects, totalTopics, totalLessons, totalQuizzes,
        //             0, 0, 0, 0,  // Progress stats are not needed for admin
        //             activeUsers,  // Active learners = active users
        //             0, // Unique quizzes attempted (not relevant for admin)
        //             mostEnrolledSubjects ?? new(),
        //             recentQuizAttempts ?? new(),
        //             topScorers?.Select(t => (t.UserId, t.TotalScore)).ToList() ?? new List<(int, double)>()
        //         );
        //     }
        //     else
        //     {
        //         // Fetch user-specific data
        //         var completedSubjects = await _dashboardRepository.GetCompletedSubjects(userId.Value);
        //         var completedTopics = await _dashboardRepository.GetCompletedTopics(userId.Value);
        //         var completedLessons = await _dashboardRepository.GetCompletedLessons(userId.Value);
        //         var completedQuizzes = await _dashboardRepository.GetCompletedQuizzes(userId.Value);
        //         var totalQuizAttempts = await _dashboardRepository.GetTotalQuizAttempts(userId.Value);
        //         var uniqueQuizzesAttempted = await _dashboardRepository.GetUniqueQuizzesAttempted(userId.Value);

        //         return DashboardMapper.ToDashboardDto(
        //             0, 0, 0,  // User-specific dashboard doesn't need total user counts
        //             0, 0, 0, 0, // User-specific dashboard doesn't need total subjects, topics, lessons
        //             completedSubjects, completedTopics, completedLessons, completedQuizzes,
        //             1, // Active learner (the logged-in user)
        //             uniqueQuizzesAttempted,
        //             new(), // No most enrolled subjects for a single user
        //             new(), // No recent quiz attempts for all users
        //             new() // No top scorers for a single user
        //         );
        //     }
        // }

    }
}
