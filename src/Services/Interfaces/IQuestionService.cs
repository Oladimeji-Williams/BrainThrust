using BrainThrust.src.Models.Entities;
using System.Threading.Tasks;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IQuestionService
    {
        Task AddQuestionAsync(Question question);
    }
}
