using BrainThrust.src.Data;
using BrainThrust.src.Models.Dtos;
using BrainThrust.src.Models.Entities;
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
        /// Create a new lesson.
        /// </summary>
        [Authorize(Roles ="Admin")]
        [HttpPost("topics/{topicId}/lessons")]
        public async Task<ActionResult<GetLessonDto>> CreateLesson(int topicId, [FromBody] CreateLessonDto createLessonDto)
        {
            if (createLessonDto == null)
            {
                _logger.LogWarning("CreateLesson called with invalId data.");
                return BadRequest(new { message = "InvalId lesson data." });
            }

            // Check if the mopic exists
            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                _logger.LogWarning("Mopic with Id {MopicId} not found.", topicId);
                return NotFound(new { message = "Topic not found." });
            }

            _logger.LogInformation("Creating a new lesson under Mopic Id={TopicId}, Title={Title}", topicId, createLessonDto.Title);

            try
            {
                var lesson = new Lesson
                {
                    Title = createLessonDto.Title,
                    Content = createLessonDto.Content,
                    VideoUrl = createLessonDto.VideoUrl,
                    TopicId = topicId
                };

                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson {LessonId} created successfully under Mopic {TopicId}", lesson.Id, topicId);

                var lessonDto = new GetLessonDto
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Content = lesson.Content,
                    VideoUrl = lesson.VideoUrl,
                    TopicId =topicId
                };

                return CreatedAtAction(nameof(GetLesson), new { Id = lesson.Id }, lessonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating lesson for Topic Id {TopicId}", topicId);
                return StatusCode(500, new { message = "An error occurred while creating the lesson." });
            }
        }

        /// <summary>
        /// Get a lesson by Id.
        /// </summary>
        [Authorize]
        [HttpGet("{Id}")]
        public async Task<ActionResult<GetLessonDto>> GetLesson(int Id)
        {
            _logger.LogInformation("Fetching lesson with Id {LessonId}", Id);

            try
            {
                var lesson = await _context.Lessons
                    .Include(l => l.Topic)
                    .FirstOrDefaultAsync(l => l.Id == Id);

                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with Id {LessonId} not found", Id);
                    return NotFound(new { message = "Lesson not found." });
                }

                var lessonDto = new GetLessonDto
                {
                    Id = lesson.Id,
                    Title = lesson.Title,
                    Content = lesson.Content,
                    VideoUrl = lesson.VideoUrl,
                    TopicId = lesson.TopicId
                };

                _logger.LogInformation("Lesson retrieved successfully: {@Lesson}", lessonDto);
                return Ok(lessonDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lesson with Id {LessonId}", Id);
                return StatusCode(500, new { message = "An error occurred while fetching the lesson." });
            }
        }

        /// <summary>
        /// Delete a lesson by Id.
        /// </summary>
        [Authorize(Roles ="Admin")]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteLesson(int Id)
        {
            _logger.LogInformation("Attempting to delete lesson with Id {LessonId}", Id);

            try
            {
                var lesson = await _context.Lessons.FindAsync(Id);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with Id {LessonId} not found for deletion", Id);
                    return NotFound();
                }

                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson {LessonId} deleted successfully", Id);
                return NoContent(); // ✅ 204 No Content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lesson with Id {LessonId}", Id);
                return StatusCode(500, new { message = "An error occurred while deleting the lesson." });
            }
        }

        /// <summary>
        /// Restore a deleted lesson.
        /// </summary>
        [Authorize(Roles ="Admin")]
        [HttpPatch("{Id}")]
        public async Task<IActionResult> UpdateLesson(int Id, [FromBody] UpdateLessonDto updateLessonDto)
        {
            _logger.LogInformation("Attempting to update lesson with Id {LessonId}", Id);

            try
            {
                var lesson = await _context.Lessons
                    .Include(l => l.Topic)
                    .FirstOrDefaultAsync(l => l.Id == Id);

                if (lesson == null)
                {
                    _logger.LogWarning("Lesson with Id {LessonId} not found", Id);
                    return NotFound(new { message = "Lesson not found." });
                }

                // If the lesson was previously deleted, restore it
                if (lesson.IsDeleted)
                {
                    lesson.IsDeleted = false;
                    _logger.LogInformation("Lesson {LessonId} was deleted, restoring it now.", Id);
                }

                // Apply updates if provIded
                if (!string.IsNullOrEmpty(updateLessonDto.Title))
                {
                    lesson.Title = updateLessonDto.Title;
                }
                if (!string.IsNullOrEmpty(updateLessonDto.Content))
                {
                    lesson.Content = updateLessonDto.Content;
                }
                if (!string.IsNullOrEmpty(updateLessonDto.VideoUrl))
                {
                    lesson.VideoUrl = updateLessonDto.VideoUrl;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Lesson {LessonId} updated successfully", Id);
                return NoContent(); // ✅ 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lesson with Id {LessonId}", Id);
                return StatusCode(500, new { message = "An error occurred while updating the lesson." });
            }
        }

    }

}