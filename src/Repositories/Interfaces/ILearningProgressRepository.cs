using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Repositories.Interfaces
{
    public interface ILearningProgressRepository
    {
        Task<bool> MarkLessonCompleted(int userId, int lessonId);
        Task<List<LessonProgress>> GetUserProgress(int userId, int subjectId);
        Task<LessonProgress?> GetLastVisitedLesson(int userId);
    }
}
