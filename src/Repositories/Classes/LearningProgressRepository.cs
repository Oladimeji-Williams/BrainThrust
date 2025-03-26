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

            // Save lesson progress
            await _context.SaveChangesAsync();

            // Check if the topic is completed
            var topicId = await _context.Lessons
                .Where(l => l.Id == lessonId)
                .Select(l => l.TopicId)
                .FirstOrDefaultAsync();

            if (topicId > 0)
            {
                await MarkTopicCompleted(userId, topicId);
            }

            return true;
        }

        private async Task MarkTopicCompleted(int userId, int topicId)
        {
            var allLessons = await _context.Lessons
                .Where(l => l.TopicId == topicId)
                .Select(l => l.Id)
                .ToListAsync();

            var completedLessons = await _context.LessonProgresses
                .Where(lp => lp.UserId == userId && lp.IsCompleted && allLessons.Contains(lp.LessonId))
                .Select(lp => lp.LessonId)
                .Distinct()
                .ToListAsync();

            if (completedLessons.Count == allLessons.Count)
            {
                var existingProgress = await _context.TopicProgresses
                    .FirstOrDefaultAsync(tp => tp.UserId == userId && tp.TopicId == topicId);

                if (existingProgress == null)
                {
                    var newProgress = new TopicProgress
                    {
                        UserId = userId,
                        TopicId = topicId,
                        IsCompleted = true,
                        DateCompleted = DateTime.UtcNow
                    };
                    await _context.TopicProgresses.AddAsync(newProgress);
                }
                else
                {
                    existingProgress.IsCompleted = true;
                    existingProgress.DateCompleted = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // Check if the subject is completed
                var subjectId = await _context.Topics
                    .Where(t => t.Id == topicId)
                    .Select(t => t.SubjectId)
                    .FirstOrDefaultAsync();

                if (subjectId > 0)
                {
                    await MarkSubjectCompleted(userId, subjectId);
                }
            }
        }

        private async Task MarkSubjectCompleted(int userId, int subjectId)
        {
            var allTopics = await _context.Topics
                .Where(t => t.SubjectId == subjectId)
                .Select(t => t.Id)
                .ToListAsync();

            var completedTopics = await _context.TopicProgresses
                .Where(tp => tp.UserId == userId && tp.IsCompleted && allTopics.Contains(tp.TopicId))
                .Select(tp => tp.TopicId)
                .Distinct()
                .ToListAsync();

            if (completedTopics.Count == allTopics.Count)
            {
                var existingProgress = await _context.SubjectProgresses
                    .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.SubjectId == subjectId);

                if (existingProgress == null)
                {
                    var newProgress = new SubjectProgress
                    {
                        UserId = userId,
                        SubjectId = subjectId,
                        IsCompleted = true,
                        DateCompleted = DateTime.UtcNow
                    };
                    await _context.SubjectProgresses.AddAsync(newProgress);
                }
                else
                {
                    existingProgress.IsCompleted = true;
                    existingProgress.DateCompleted = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
        }

        // Add missing methods to implement the interface
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


