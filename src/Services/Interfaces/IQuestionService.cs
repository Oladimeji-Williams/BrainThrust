using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IQuestionService
    {
        Task AddQuestionAsync(Question question);
    }
}
