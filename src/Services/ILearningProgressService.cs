using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Services
{
    public interface ILearningProgressService
    {
        Task<bool> MarkLessonCompleted(int lessonId, int userId);
        Task<List<LessonProgress>> GetUserProgress(int subjectId, int userId);
        Task<LessonProgress?> GetLastVisitedLesson(int userId);
        Task<Lesson?> GetLessonByIdAsync(int lessonId);
        Task<bool> IsUserEnrolledInSubject(int userId, int subjectId);
        Task<List<Lesson>> GetPreviousLessons(int lessonId, int topicId);
        Task<List<int>> GetUserCompletedLessons(int userId, int topicId);
        Task<List<Topic>> GetPreviousTopics(int topicId, int subjectId);
        Task<List<Lesson>> GetLessonsByTopic(int topicId);
        Task<Quiz> GetQuizByTopicId(int topicId);
        Task<UserQuizSubmission> GetUserQuizSubmission(int userId, int quizId);
    }

}
