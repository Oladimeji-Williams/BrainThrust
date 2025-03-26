using BrainThrust.src.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Repositories.Interfaces;

namespace BrainThrust.src.Repositories.Classes
{
    public class LearningProgressRepository : ILearningProgressRepository
    {
        private readonly ApplicationDbContext _context;

        public LearningProgressRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> MarkLessonCompleted(int userId, int lessonId)
        {
            var existingProgress = await _context.LessonProgresses
                .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == lessonId);

            if (existingProgress != null)
            {
                existingProgress.IsCompleted = true;
                existingProgress.DateCompleted = DateTime.UtcNow;
            }
            else
            {
                var newProgress = new LessonProgress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    IsCompleted = true,
                    DateCompleted = DateTime.UtcNow
                };
                await _context.LessonProgresses.AddAsync(newProgress);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<LessonProgress>> GetUserProgress(int userId, int subjectId)
        {
            return await _context.LessonProgresses
                .Include(lp => lp.Lesson)
                .ThenInclude(l => l.Topic)
                .Where(lp => lp.UserId == userId && lp.Lesson.Topic.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<LessonProgress?> GetLastVisitedLesson(int userId)
        {
            return await _context.LessonProgresses
                .Include(lp => lp.Lesson)
                .Where(lp => lp.UserId == userId)
                .OrderByDescending(lp => lp.DateCompleted)
                .FirstOrDefaultAsync();
        }
    }
}
