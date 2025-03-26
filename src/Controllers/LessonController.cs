using BrainThrust.src.Dtos.LessonDtos;
using BrainThrust.src.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/lessons")]
    public class LessonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LessonController> _logger;

        public LessonController(ApplicationDbContext context, ILogger<LessonController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get a lesson by Id.
        /// </summary>
        [Authorize]
        [HttpGet("{Id}")]
        public async Task<ActionResult<GetLessonDto>> GetLesson(int Id)
        {
            var lessonEntity = await _context.Lessons
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == Id && !l.IsDeleted); // ✅ Filter first

            if (lessonEntity == null)
            {
                _logger.LogWarning("GetLesson: Lesson with Id {Id} not found.", Id);
                return NotFound("Lesson not found.");
            }

            var lessonDto = lessonEntity.ToLessonDto(); // ✅ Convert to DTO after retrieval

            _logger.LogInformation("GetLesson: Retrieved lesson '{Title}' with Id {Id}.", lessonDto.Title, lessonDto.Id);
            return Ok(lessonDto);
        }


        /// <summary>
        /// Create a new lesson.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("topics/{topicId}/lessons")]
        public async Task<IActionResult> CreateLesson(int topicId, [FromBody] CreateLessonDto createLessonDto)
        {
            if (createLessonDto == null)
            {
                _logger.LogWarning("CreateLesson: Invalid lesson data received.");
                return BadRequest("Lesson data is required.");
            }

            var topicExists = await _context.Topics.AnyAsync(t => t.Id == topicId);
            if (!topicExists)
            {
                _logger.LogWarning("CreateLesson: Topic with Id {TopicId} not found.", topicId);
                return NotFound(new { message = "Topic not found." });
            }

            var lesson = createLessonDto.ToLesson(topicId);

            try
            {
                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CreateLesson: Lesson '{Title}' created with Id {Id}.", lesson.Title, lesson.Id);

                var createdLessonDto = lesson.ToLessonDto();
                return CreatedAtAction(nameof(GetLesson), new { Id = lesson.Id }, createdLessonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateLesson: Error creating lesson for TopicId {TopicId}.", topicId);
                return StatusCode(500, "An error occurred while creating the lesson.");
            }
        }



        /// <summary>
        /// Update an existing lesson.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("{Id}")]
        public async Task<IActionResult> UpdateLesson(int Id, [FromBody] UpdateLessonDto updateLessonDto)
        {
            _logger.LogInformation("Attempting to update lesson with Id {LessonId}", Id);

            try
            {
                var lesson = await _context.Lessons.FindAsync(Id);

                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with Id {LessonId} not found", Id);
                    return NotFound(new { message = "Lesson not found." });
                }

                // Restore soft-deleted lessons
                if (lesson.IsDeleted)
                {
                    lesson.IsDeleted = false;
                    lesson.DateDeleted = null;
                    _logger.LogInformation("Lesson {LessonId} was deleted, restoring it now.", Id);
                }

                lesson.UpdateLessonFromDto(updateLessonDto);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson {LessonId} updated successfully", Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson with Id {LessonId}", Id);
                return StatusCode(500, new { message = "An error occurred while updating the lesson." });
            }
        }

        /// <summary>
        /// Delete a lesson by Id.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteLesson(int Id)
        {
            _logger.LogInformation("Attempting to delete lesson with Id {LessonId}", Id);

            var lesson = await _context.Lessons.FindAsync(Id);
            if (lesson == null)
            {
                _logger.LogWarning("Lesson with Id {LessonId} not found for deletion", Id);
                return NotFound("Lesson not found.");
            }

            try
            {
                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson {LessonId} deleted successfully", Id);
                return Ok($"Lesson '{lesson.Title}' has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson with Id {LessonId}", Id);
                return StatusCode(500, "An error occurred while deleting the lesson.");
            }
        }
    }
}
