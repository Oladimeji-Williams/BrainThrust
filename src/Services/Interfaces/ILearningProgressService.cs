using BrainThrust.src.Dtos.LearningProgressDtos;

namespace BrainThrust.src.Services.Interfaces
{
    public interface ILearningProgressService
    {
        Task<bool> MarkLessonCompleted(int userId, int lessonId);
        Task<List<LearningProgressDto>> GetUserProgress(int userId, int subjectId);
        Task<LearningProgressDto?> GetLastVisitedLesson(int userId);
    }
}
