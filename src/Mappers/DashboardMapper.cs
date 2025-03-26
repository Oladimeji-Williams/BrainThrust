using System.Collections.Generic;
using System.Linq;
using BrainThrust.src.Dtos.ReportDtos;

namespace BrainThrust.src.Mappers
{
    public static class DashboardMapper
    {
        public static DashboardDto ToDashboardDto(
            int totalUsers, int activeUsers, int newUsers,
            int totalSubjects, int totalTopics, int totalLessons, int totalQuizzes,
            int completedSubjects, int completedTopics, int completedLessons, int completedQuizzes,
            int activeLearners, List<(int SubjectId, int Enrollments)> mostEnrolledSubjects,
            List<(int UserId, int QuizId, int TotalScore, bool IsPassed, DateTime Created)> recentQuizAttempts,
            List<(int UserId, int TotalScore)> topScorers)
        {
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
                    TotalTopics = totalTopics,
                    TotalLessons = totalLessons,
                    TotalQuizzes = totalQuizzes
                },
                Progress = new ProgressStatsDto
                {
                    CompletedSubjects = completedSubjects,
                    CompletedTopics = completedTopics,
                    CompletedLessons = completedLessons,
                    CompletedQuizzes = completedQuizzes
                },
                
                Engagement = new EngagementStatsDto
                {
                    ActiveLearners = activeLearners,
                    MostEnrolledSubjects = mostEnrolledSubjects
                        .Select(s => new MostEnrolledSubjectDto { SubjectId = s.SubjectId, Enrollments = s.Enrollments })
                        .ToList()
                },
                QuizPerformance = new QuizPerformanceDto
                {
                    RecentQuizAttempts = recentQuizAttempts
                        .Select(q => new RecentQuizAttemptDto
                        {
                            UserId = q.UserId,
                            QuizId = q.QuizId,
                            TotalScore = q.TotalScore,
                            IsPassed = q.IsPassed,
                            Created = q.Created
                        }).ToList(),
                    TopScorers = topScorers
                        .Select(q => new TopScorerDto
                        {
                            UserId = q.UserId,
                            TotalScore = q.TotalScore
                        }).ToList()
                }
            };
        }
    }
}
