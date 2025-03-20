using BrainThrust.src.Models.Dtos;
using BrainThrust.src.Models.Entities;
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

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromBody] TopicDto topicDto)
        {
            if (topicDto == null)
            {
                _logger.LogWarning("CreateTopic: Received null topic data.");
                return BadRequest("Topic data is required.");
            }
            var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == topicDto.SubjectId);
            if (!subjectExists)
            {
                _logger.LogWarning("CreateTopic: Subject with Id {SubjectId} does not exist.", topicDto.SubjectId);
                return BadRequest($"Subject with Id {topicDto.SubjectId} does not exist.");
            }

            var topic = new Topic
            {
                Title = topicDto.Title,
                Description = topicDto.Description,
                SubjectId = topicDto.SubjectId
            };

            try
            {
                _context.Topics.Add(topic);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CreateTopic: Topic '{Title}' created with Id {Id}.", topic.Title, topic.Id);

                var createdTopicDto = new GetTopicDto
                {
                    Id = topic.Id,
                    Title = topic.Title,
                    Description = topic.Description,
                    SubjectId = topic.SubjectId,
                    LessonIds = new List<int>()
                };

                return CreatedAtAction(nameof(GetTopic), new { Id = topic.Id }, createdTopicDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateTopic: Error creating topic '{Title}'.", topic.Title);
                return StatusCode(500, "An error occurred while creating the topic.");
            }
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<GetTopicDto>> GetTopic(int Id)
        {
            var topic = await _context.Topics
                .Include(m => m.Lessons)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == Id);

            if (topic == null)
            {
                _logger.LogWarning("GetTopic: Topic with Id {Id} not found.", Id);
                return NotFound("Topic not found.");
            }

            var getTopicDto = new GetTopicDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                SubjectId = topic.SubjectId,
                LessonIds = topic.Lessons.Select(l => l.Id).ToList()
            };

            _logger.LogInformation("GetTopic: Retrieved topic '{Title}' with Id {Id}.", topic.Title, topic.Id);
            return Ok(getTopicDto);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetTopicDto>>> GetAllTopics([FromQuery] bool includeDeleted = false)
        {
            IQueryable<Topic> query = _context.Topics;

            if (!includeDeleted)
                query = query.Where(m => !m.IsDeleted);

            var topics = await query
                .Include(m => m.Lessons)
                .AsNoTracking()
                .ToListAsync();

            var topicDtos = topics.Select(topic => new GetTopicDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                SubjectId = topic.SubjectId,
                LessonIds = topic.Lessons.Select(l => l.Id).ToList()
            }).ToList();

            _logger.LogInformation("GetAllTopics: Retrieved {Count} topics (includeDeleted: {IncludeDeleted}).", topicDtos.Count, includeDeleted);
            return Ok(topicDtos);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{Id}")]
        public async Task<IActionResult> UpdateTopic(int Id, [FromBody] UpdateTopicDto topicDto)
        {
            _logger.LogInformation("Attempting to update topic with Id {TopicId}", Id);

            try
            {
                var topic = await _context.Topics.FindAsync(Id);

                if (topic == null)
                {
                    _logger.LogWarning("Topic with Id {TopicId} not found", Id);
                    return NotFound(new { message = "Topic not found." });
                }

                // Restore soft-deleted topics
                if (topic.IsDeleted)
                {
                    topic.IsDeleted = false;
                    topic.DateDeleted = null;
                    _logger.LogInformation("Topic {TopicId} was deleted, restoring it now.", Id);
                }

                // Apply updates only if provIded
                if (!string.IsNullOrEmpty(topicDto.Title))
                    topic.Title = topicDto.Title;

                if (!string.IsNullOrEmpty(topicDto.Description))
                    topic.Description = topicDto.Description;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Topic {TopicId} updated successfully", Id);
                return NoContent(); // ✅ 204 No Content for successful updates
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topic with Id {TopicId}", Id);
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