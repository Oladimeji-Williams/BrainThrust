using BrainThrust.src.Models.Entities;
using BrainThrust.src.Services.Interfaces;

namespace BrainThrust.src.Services.Classes
{
    public class QuestionService : IQuestionService
    {
        private readonly ApplicationDbContext _context;

        public QuestionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddQuestionAsync(Question question)
        {
            ValidateQuestionOptions(question);
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        private static void ValidateQuestionOptions(Question question)
        {
            if (question.Options == null || question.Options.Count < 3)
            {
                throw new ArgumentException("A question must have at least 3 options.");
            }

            if (question.Options.Count > 5)
            {
                throw new ArgumentException("A question can have at most 5 options.");
            }

            if (question.CorrectOptionId == null || !question.Options.Any(o => o.Id == question.CorrectOptionId))
            {
                throw new ArgumentException("The correct option must be one of the question's options.");
            }
        }
    }
}
