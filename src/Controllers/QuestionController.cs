using BrainThrust.src.Dtos.QuestionDtos;
using BrainThrust.src.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/questions")]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<QuestionController> _logger;

        public QuestionController(ApplicationDbContext context, ILogger<QuestionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET ALL QUESTIONS
        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            var questionsDto = await _context.Questions
                .Include(q => q.Options)
                .Select(q => q.ToQuestionDto())
                .ToListAsync();

            _logger.LogInformation("GetQuestions: Retrieved {Count} questions.", questionsDto.Count);
            return Ok(questionsDto);
        }

        // GET SINGLE QUESTION BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<GetQuestionDto>> GetQuestion(int id)
        {
            var questionDto = await _context.Questions
                .Include(q => q.Options)
                .AsNoTracking()
                .Where(q => q.Id == id)
                .Select(q => q.ToQuestionDto())
                .FirstOrDefaultAsync();

            if (questionDto == null)
            {
                _logger.LogWarning("GetQuestion: Question with Id {Id} not found.", id);
                return NotFound("Question not found.");
            }

            _logger.LogInformation("GetQuestion: Retrieved question with Id {Id}.", id);
            return Ok(questionDto);
        }

        // CREATE QUESTION
        [Authorize(Roles = "Admin")]
        [HttpPost("{quizId}")]
        public async Task<IActionResult> CreateQuestion(int quizId, [FromBody] CreateQuestionDto createQuestionDto)
        {
            if (createQuestionDto == null)
            {
                _logger.LogWarning("CreateQuestion: Received null question data.");
                return BadRequest("Question data is required.");
            }

            // Validate if Quiz exists
            var isQuizExists = await _context.Quizzes.AnyAsync(q => q.Id == quizId);
            if (!isQuizExists)
            {
                _logger.LogWarning("CreateQuestion: Quiz with Id {QuizId} does not exist.", quizId);
                return BadRequest($"Quiz with Id {quizId} does not exist.");
            }

            var question = createQuestionDto.ToQuestion(quizId);

            try
            {
                // Step 1: Add Question to Database
                _context.Questions.Add(question);
                await _context.SaveChangesAsync();

                // Step 2: Retrieve the correct option from the database and update CorrectOptionId
                var correctOption = question.Options.FirstOrDefault(o => o.Text == createQuestionDto.CorrectOption);
                if (correctOption != null)
                {
                    question.CorrectOptionId = correctOption.Id;
                    await _context.SaveChangesAsync(); // Save the updated correct option ID
                }

                _logger.LogInformation("CreateQuestion: Question '{Text}' created with Id {Id}.", question.QuestionText, question.Id);

                var createdQuestionDto = question.ToQuestionDto();
                return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, createdQuestionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateQuestion: Error creating question.");
                return StatusCode(500, "An error occurred while creating the question.");
            }
        }


        // UPDATE QUESTION
        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionDto questionDto)
        {
            _logger.LogInformation("Attempting to update question with Id {Id}", id);

            try
            {
                var question = await _context.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);

                if (question == null)
                {
                    _logger.LogWarning("UpdateQuestion: Question with Id {Id} not found", id);
                    return NotFound(new { message = "Question not found." });
                }

                question.UpdateQuestionFromDto(questionDto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("UpdateQuestion: Question {Id} updated successfully", id);
                return NoContent(); // ✅ 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating question with Id {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the question." });
            }
        }

        // DELETE QUESTION
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                _logger.LogWarning("DeleteQuestion: Question with Id {Id} not found.", id);
                return NotFound("Question not found.");
            }

            try
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();

                _logger.LogInformation("DeleteQuestion: Question with Id {Id} deleted.", id);
                return Ok($"Question has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteQuestion: Error deleting question with Id {Id}.", id);
                return StatusCode(500, "An error occurred while deleting the question.");
            }
        }
    }
}
