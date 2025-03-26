using BrainThrust.src.Dtos.TopicDtos;
using BrainThrust.src.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TopicController> _logger;

        public TopicController(ApplicationDbContext context, ILogger<TopicController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetTopics()
        {

            var topicsDto = await _context.Topics
                .Where(t => !t.IsDeleted)  // Exclude deleted topics
                .Select(t => t.ToTopicDto())
                .ToListAsync();

            _logger.LogInformation("GetAllTopics: Retrieved {Count} topics).", topicsDto.Count);
            return Ok(topicsDto);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetTopicDto>> GetTopic(int Id)
        {
            var topicEntity = await _context.Topics
                .Include(t => t.Lessons)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == Id); // ✅ Apply filter first in SQL

            if (topicEntity == null)
            {
                _logger.LogWarning("GetTopic: Topic with Id {Id} not found.", Id);
                return NotFound("Topic not found.");
            }

            var topicDto = topicEntity.ToTopicDto(); // ✅ Convert to DTO after fetching

            _logger.LogInformation("GetTopic: Retrieved topic '{Title}' with Id {Id}.", topicDto.Title, topicDto.Id);
            return Ok(topicDto);
        }


        [Authorize(Roles ="Admin")]
        [HttpPost("{subjectId}")]
        public async Task<IActionResult> CreateTopic(int subjectId, [FromBody] CreateTopicDto createTopicDto)
        {
            if (createTopicDto == null)
            {
                _logger.LogWarning("CreateTopic: Received null topic data.");
                return BadRequest("Topic data is required.");
            }
            var isSubject = await _context.Subjects.AnyAsync(s => s.Id == subjectId);
            if (!isSubject)
            {
                _logger.LogWarning("CreateTopic: Subject with Id {SubjectId} does not exist.", subjectId);
                return BadRequest($"Subject with Id {subjectId} does not exist.");
            }

            var topic = createTopicDto.ToTopic(subjectId);

            try
            {
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CreateTopic: Topic '{Title}' created with Id {Id}.", topic.Title, topic.Id);

                var createdTopicDto = topic.ToTopicDto();

                return CreatedAtAction(nameof(GetTopic), new { Id = topic.Id }, createdTopicDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTopic: Error creating topic '{Title}'.", topic.Title);
                return StatusCode(500, "An error occurred while creating the topic.");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{topicId}")]
        public async Task<IActionResult> UpdateTopic(int topicId, [FromBody] UpdateTopicDto topicDto)
        {
            _logger.LogInformation("Attempting to update topic with Id {TopicId}", topicId);

            try
            {
                var topic = await _context.Topics.FindAsync(topicId);

                if (topic == null)
                {
                    _logger.LogWarning("Topic with Id {TopicId} not found", topicId);
                    return NotFound(new { message = "Topic not found." });
                }

                // Restore soft-deleted topics
                if (topic.IsDeleted)
                {
                    topic.IsDeleted = false;
                    topic.DateDeleted = null;
                    _logger.LogInformation("Topic {TopicId} was deleted, restoring it now.", topicId);
                }


                topic.UpdateTopicFromDto(topicDto);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Topic {TopicId} updated successfully", topicId);
                return NoContent(); // ✅ 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topic with Id {TopicId}", topicId);
                return StatusCode(500, new { message = "An error occurred while updating the topic." });
            }
        }

        [HttpDelete("{Id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteTopic(int Id)
        {
            var topic = await _context.Topics
                .Include(m => m.Lessons)
                .Include(m => m.Quiz)
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (topic == null)
            {
                _logger.LogWarning("DeleteTopic: Topic with Id {Id} not found.", Id);
                return NotFound("Topic not found.");
            }

            try
            {
                if (topic.Lessons != null && topic.Lessons.Count != 0)
                    _context.Lessons.RemoveRange(topic.Lessons);

                if (topic.Quiz != null)
                    _context.Quizzes.Remove(topic.Quiz);

                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();

                _logger.LogInformation("DeleteTopic: Topic '{Title}' with Id {Id} deleted.", topic.Title, Id);
                return Ok($"Topic '{topic.Title}' has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteTopic: Error deleting topic with Id {Id}.", Id);
                return StatusCode(500, "An error occurred while deleting the topic.");
            }
        }

    }

}