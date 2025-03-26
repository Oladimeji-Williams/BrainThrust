using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BrainThrust.src.Services.Interfaces;
using BrainThrust.src.Services.Classes;

namespace BrainThrust.src.Controllers
{
    [Route("api/progress")]
    [ApiController]
    public class LearningProgressController : ControllerBase
    {
        private readonly ILearningProgressService _learningProgressService;
        private readonly ILogger<LearningProgressController> _logger;
        private readonly UserService _userService;

        public LearningProgressController(ILearningProgressService learningProgressService, 
                                          ILogger<LearningProgressController> logger,
                                          UserService userService)
        {
            _learningProgressService = learningProgressService;
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Marks a lesson as completed.
        /// </summary>
        [Authorize]
        [HttpPost("lesson/{lessonId}/complete")]
        public async Task<IActionResult> MarkLessonCompleted([FromRoute] int lessonId)
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("MarkLessonCompleted: Unauthorized access attempt.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            try
            {
                bool isMarked = await _learningProgressService.MarkLessonCompleted(userId.Value, lessonId);
                if (isMarked)
                {
                    _logger.LogInformation("MarkLessonCompleted: User {UserId} marked Lesson {LessonId} as completed.", userId, lessonId);
                    return Ok(new { message = "Lesson marked as completed!" });
                }
                else
                {
                    _logger.LogWarning("MarkLessonCompleted: Failed to mark Lesson {LessonId} for User {UserId}.", lessonId, userId);
                    return NotFound(new { message = "Failed to mark lesson as completed." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MarkLessonCompleted: Error occurred while marking Lesson {LessonId} for User {UserId}.", lessonId, userId);
                return StatusCode(500, new { message = "An error occurred while marking the lesson as completed." });
            }
        }

        /// <summary>
        /// Gets the user's progress for a specific subject.
        /// </summary>
        [Authorize]
        [HttpGet("subject/{subjectId}")]
        public async Task<IActionResult> GetUserProgress([FromRoute] int subjectId)
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("GetUserProgress: Unauthorized access attempt.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            try
            {
                var progress = await _learningProgressService.GetUserProgress(userId.Value, subjectId);
                if (progress.Count > 0)
                {
                    _logger.LogInformation("GetUserProgress: Retrieved progress for User {UserId}, Subject {SubjectId}.", userId, subjectId);
                    return Ok(progress);
                }
                else
                {
                    _logger.LogWarning("GetUserProgress: No progress found for User {UserId}, Subject {SubjectId}.", userId, subjectId);
                    return NotFound(new { message = "No progress found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserProgress: Error retrieving progress for User {UserId}, Subject {SubjectId}.", userId, subjectId);
                return StatusCode(500, new { message = "An error occurred while retrieving progress." });
            }
        }

        /// <summary>
        /// Gets the last lesson the user visited.
        /// </summary>
        [Authorize]
        [HttpGet("user/last-lesson")]
        public async Task<IActionResult> GetLastLesson()
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("GetLastLesson: Unauthorized access attempt.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            try
            {
                var lastLesson = await _learningProgressService.GetLastVisitedLesson(userId.Value);
                if (lastLesson != null)
                {
                    _logger.LogInformation("GetLastLesson: Retrieved last visited lesson for User {UserId}.", userId);
                    return Ok(new { lastLesson });
                }
                else
                {
                    _logger.LogWarning("GetLastLesson: No last lesson found for User {UserId}.", userId);
                    return NotFound(new { message = "No lesson progress found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLastLesson: Error retrieving last visited lesson for User {UserId}.", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving the last visited lesson." });
            }
        }
    }
}
