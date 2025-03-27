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
        public async Task<int> GetTotalTopics() => await _context.Topics.CountAsync();
        public async Task<int> GetTotalLessons() => await _context.Lessons.CountAsync();
        public async Task<int> GetTotalQuizzes() => await _context.Quizzes.CountAsync();

        public async Task<List<MostEnrolledSubjectDto>> GetMostEnrolledSubjects()
        {
            return await _context.Enrollments
                .GroupBy(e => e.SubjectId)
                .Select(g => new
                {
                    SubjectId = g.Key,
                    Enrollments = g.Count()
                })
                .OrderByDescending(g => g.Enrollments)
                .Take(5)
                .Join(_context.Subjects,
                    e => e.SubjectId,
                    s => s.Id,
                    (e, s) => new MostEnrolledSubjectDto
                    {
                        SubjectId = e.SubjectId,
                        SubjectName = s.Title,
                        Enrollments = e.Enrollments
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




        public async Task<int> GetCompletedSubjects(int userId)
        {
            return await _context.SubjectProgresses
                .Where(us => us.UserId == userId && us.IsCompleted)
                .CountAsync();
        }

        public async Task<int> GetCompletedTopics(int userId)
        {
            return await _context.TopicProgresses
                .Where(ut => ut.UserId == userId && ut.IsCompleted)
                .CountAsync();
        }

        public async Task<int> GetCompletedLessons(int userId)
        {
            return await _context.LessonProgresses
                .Where(ul => ul.UserId == userId && ul.IsCompleted)
                .CountAsync();
        }

        public async Task<int> GetQuizzes(int userId)
        {
            return await _context.UserQuizAttempts
                .Where(uq => uq.UserId == userId && uq.IsPassed)
                .CountAsync();
        }

        public async Task<int> GetTotalQuizAttempts(int userId)
        {
            return await _context.UserQuizAttempts
                .Where(uq => uq.UserId == userId)
                .CountAsync();
        }
        public async Task<int> GetCompletedQuizzes(int userId)
        {
            return await _context.UserQuizAttempts
                .Where(uq => uq.UserId == userId && uq.IsPassed)
                .CountAsync();
        }

        public async Task<int> GetUniqueQuizzesAttempted(int userId)
        {
            return await _context.UserQuizAttempts
                .Where(uq => uq.UserId == userId)
                .Select(uq => uq.QuizId)
                .Distinct()
                .CountAsync();
        }

    }
}
