using BrainThrust.src.Dtos.QuizDtos;
using BrainThrust.src.Dtos.SubmissionDtos;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IQuizService
    {
        Task<bool> TopicExists(int topicId);
        Task<QuizDto> CreateQuiz(CreateQuizDto dto);
        Task<QuizDto> GetQuizByTopicId(int topicId);
        Task<bool> UpdateQuizByTopicId(int topicId, UpdateQuizDto dto);
        Task<bool> DeleteQuizByTopicId(int topicId);
        Task<QuizDto> TakeQuizByTopic(int topicId, int userId);
        Task<SubmitQuizResponseDto> SubmitQuiz(SubmitQuizDto submission, int userId);
    }
}