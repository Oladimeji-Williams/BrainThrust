using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Data;
using BrainThrust.src.Models.Entities;
using BrainThrust.src.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace BrainThrust.src.Controllers
{
[Route("api/quizzes/{quizId}/questions")]
[ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQuestion(int quizId, [FromBody] CreateQuestionDto questionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
            {
                return NotFound("Quiz not found");
            }

            // Create the question entity
            var question = new Question
            {
                QuizId = quizId,
                QuestionText = questionDto.QuestionText,
                Options = questionDto.Options.Select(o => new Option { Text = o.Text }).ToList()
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();  // Save first so the Question gets an ID

            // Assign the correct option ID
            var correctOption = question.Options.FirstOrDefault(o => o.Text.Equals(questionDto.CorrectOption, StringComparison.OrdinalIgnoreCase));
            if (correctOption == null)
            {
                return BadRequest("Correct option must match one of the provided options.");
            }

            question.CorrectOptionId = correctOption.Id;

            await _context.SaveChangesAsync();  // Save again to update the correct option ID

            return CreatedAtAction(nameof(GetQuestionById), new { quizId, id = question.Id }, question);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestionById(int quizId, int id)
        {
            var question = _context.Questions
                .Include(q => q.Quiz)  // Ensure related quiz is loaded
                .Include(q => q.Options) // Ensure options are loaded
                .FirstOrDefault(q => q.Id == id && q.QuizId == quizId);

            if (question == null)
            {
                return NotFound();
            }

            return Ok(question);
        }
    }
}
