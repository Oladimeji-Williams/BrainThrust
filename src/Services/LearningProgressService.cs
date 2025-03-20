using BrainThrust.src.Data;
using BrainThrust.src.Models.Entities;
using BrainThrust.src.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class LearningProgressService : ILearningProgressService
{
    private readonly ApplicationDbContext _context;

    public LearningProgressService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Marks a lesson as completed for a user.
    /// </summary>
    public async Task<bool> MarkLessonCompleted(int lessonId, int userId)
    {
        var lessonProgress = await _context.LessonProgresses
            .FirstOrDefaultAsync(lp => lp.LessonId == lessonId && lp.UserId == userId);

        if (lessonProgress == null)
        {
            lessonProgress = new LessonProgress
            {
                LessonId = lessonId,
                UserId = userId,
                IsCompleted = true,
                DateCompleted = DateTime.UtcNow
            };
            _context.LessonProgresses.Add(lessonProgress);
        }
        else if (!lessonProgress.IsCompleted)
        {
            lessonProgress.IsCompleted = true;
            lessonProgress.DateCompleted = DateTime.UtcNow;
            _context.LessonProgresses.Update(lessonProgress);
        }
        else
        {
            // If already completed, do nothing
            return true;
        }

        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Retrieves the progress of a user in a specific subject.
    /// </summary>
    public async Task<List<LessonProgress>> GetUserProgress(int subjectId, int userId)
    {
        return await _context.LessonProgresses
            .Include(lp => lp.Lesson)
                .ThenInclude(lesson => lesson.Topic)
            .Where(lp => lp.UserId == userId && lp.Lesson.Topic.SubjectId == subjectId)
            .ToListAsync();
    }

    /// <summary>
    /// Gets the last visited lesson for a user.
    /// </summary>
    public async Task<LessonProgress?> GetLastVisitedLesson(int userId)
    {
        return await _context.LessonProgresses
            .Where(lp => lp.UserId == userId)
            .OrderByDescending(lp => lp.DateCompleted)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a lesson by its Id.
    /// </summary>
    public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
    {
        return await _context.Lessons
            .Include(l => l.Topic) // Ensure Topic is included for subject reference
                .ThenInclude(t => t.Subject)
            .FirstOrDefaultAsync(l => l.Id == lessonId);
    }

    /// <summary>
    /// Checks if a user is enrolled in a subject.
    /// </summary>
    public async Task<bool> IsUserEnrolledInSubject(int userId, int subjectId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.UserId == userId && e.SubjectId == subjectId && !e.IsDeleted);
    }
    public async Task<List<Lesson>> GetPreviousLessons(int lessonId, int topicId)
    {
        return await _context.Lessons
            .Where(l => l.TopicId == topicId && l.Id < lessonId)
            .OrderBy(l => l.Id)
            .ToListAsync();
    }

    public async Task<List<int>> GetUserCompletedLessons(int userId, int topicId)
    {
        return await _context.LessonProgresses
            .Where(lp => lp.UserId == userId && lp.Lesson.TopicId == topicId && lp.IsCompleted)
            .Select(lp => lp.LessonId)
            .ToListAsync();
    }

    public async Task<List<Topic>> GetPreviousTopics(int topicId, int subjectId)
    {
        return await _context.Topics
            .Where(t => t.SubjectId == subjectId && t.Id < topicId)
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task<List<Lesson>> GetLessonsByTopic(int topicId)
    {
        return await _context.Lessons
            .Where(l => l.TopicId == topicId)
            .OrderBy(l => l.Id)
            .ToListAsync();
    }
    // ✅ Fetch quiz for a specific topic
    public async Task<Quiz> GetQuizByTopicId(int topicId)
    {
        return await _context.Quizzes
            .Include(q => q.Topic) // Ensure topic is included
            .Include(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.TopicId == topicId);
    }

    // ✅ Fetch user’s quiz attempt for a specific quiz
    public async Task<UserQuizSubmission> GetUserQuizSubmission(int userId, int quizId)
    {
        return await _context.UserQuizSubmissions
            .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
            .OrderByDescending(qa => qa.Created) // Get the latest attempt
            .FirstOrDefaultAsync();
    }

}
