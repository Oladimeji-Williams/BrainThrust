using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Dtos.ReportDtos;
using BrainThrust.src.Repositories.Interfaces;

namespace BrainThrust.src.Repositories.Classes
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;

        public DashboardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalUsers() => await _context.Users.CountAsync();
        public async Task<int> GetActiveUsers() => await _context.Users.Where(u => !u.IsDeleted).CountAsync();
        public async Task<int> GetNewUsers() => await _context.Users.Where(u => u.Created >= DateTime.UtcNow.AddDays(-30)).CountAsync();
        public async Task<int> GetTotalSubjects() => await _context.Subjects.CountAsync();


        public async Task<List<MostEnrolledSubjectDto>> GetMostEnrolledSubjects()
        {
            return await _context.Enrollments
                .GroupBy(e => e.SubjectId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new MostEnrolledSubjectDto
                {
                    SubjectId = g.Key,
                    Enrollments = g.Count()
                })
                .ToListAsync();
        }

        /// <summary>
        /// Fetches the most recent quiz attempts.
        /// </summary>
        public async Task<List<(int UserId, int QuizId, int TotalScore, bool IsPassed, DateTime Created)>> GetRecentQuizAttempts()
        {
            var quizAttempts = await _context.UserQuizAttempts
                .OrderByDescending(q => q.Created)
                .Take(10)
                .Select(q => new ValueTuple<int, int, int, bool, DateTime>(
                    q.UserId, q.QuizId, (int)q.TotalScore, q.IsPassed, q.Created
                ))
                .ToListAsync();

            Console.WriteLine($"Fetched {quizAttempts.Count} recent quiz attempts.");
            return quizAttempts;
        }


        /// <summary>
        /// Fetches the top scorers from quiz attempts.
        /// </summary>
        public async Task<List<TopScorerDto>> GetTopScorers()
        {
            var scorers = await _context.UserQuizAttempts
                .Where(u => !u.IsDeleted)
                .GroupBy(q => q.UserId)
                .Select(g => new TopScorerDto
                {
                    UserId = g.Key,
                    TotalScore = (int)g.Sum(q => q.TotalScore)
                })
                .OrderByDescending(g => g.TotalScore)
                .Take(5)
                .ToListAsync();

            Console.WriteLine($"Fetched {scorers.Count} top scorers.");
            return scorers;

        }

    }
}
