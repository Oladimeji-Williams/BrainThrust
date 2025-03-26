using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Repositories.Interfaces
{
    public interface IQuestionRepository
    {
        Task AddQuestionAsync(Question question);
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task<List<Question>> GetAllQuestionsAsync();
    }
}