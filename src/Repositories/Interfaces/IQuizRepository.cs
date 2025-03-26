using BrainThrust.src.Dtos.QuizDtos;
using BrainThrust.src.Dtos.SubmissionDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Repositories.Interfaces
{
    public interface IQuizRepository
    {

        Task<Quiz> GetQuizByTopicId(int topicId);
        Task<Quiz> CreateQuiz(Quiz quiz);
        Task<bool> UpdateQuiz(Quiz quiz);
        Task<bool> DeleteQuiz(int topicId);
        Task<SubmitQuizResponseDto> SubmitQuiz(SubmitQuizDto submission, int userId);
    }
}

