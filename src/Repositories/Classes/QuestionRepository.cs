using BrainThrust.src.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Repositories.Interfaces;

namespace BrainThrust.src.Repositories.Classes
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            return await _context.Questions.FindAsync(questionId);
        }

        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            return await _context.Questions.ToListAsync();
        }
    }
}