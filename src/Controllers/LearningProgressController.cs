using Microsoft.AspNetCore.Mvc;
using BrainThrust.src.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BrainThrust.src.Controllers
{
    [Route("api/progress")]
    [ApiController]
    public class LearningProgressController : ControllerBase
    {
        private readonly ILearningProgressService _learningProgressService;
        private readonly ILogger<LearningProgressController> _logger;
        private readonly UserService _userService; // Inject UserService
        public LearningProgressController(ILearningProgressService learningProgressService, 
                                          ILogger<LearningProgressController> logger,
                                          UserService userService) // Inject UserService
        {
            _learningProgressService = learningProgressService;
            _logger = logger;
            _userService = userService;
        }
   

        /// <summary>
        /// Marks a lesson as completed for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpPost("lesson/{lessonId}/complete")]
        public async Task<IActionResult> MarkLessonCompleted([FromRoute] int lessonId)
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null) return Unauthorized(new { message = "User not authenticated." });

            try
            {
                _logger.LogInformation("Fetching lesson {LessonId} for user {UserId}", lessonId, userId);

                var lesson = await _learningProgressService.GetLessonByIdAsync(lessonId);
                if (lesson == null)
                {
                    _logger.LogWarning("Lesson {LessonId} not found", lessonId);
                    return NotFound(new { message = "Lesson not found." });
                }

                if (lesson.Topic == null)
                {
                    _logger.LogError("Lesson {LessonId} has no associated topic", lessonId);
                    return BadRequest(new { message = "Lesson topic not found." });
                }

                var isUserEnrolled = await _learningProgressService.IsUserEnrolledInSubject(userId.Value, lesson.Topic.SubjectId);
                if (!isUserEnrolled)
                {
                    return StatusCode(403, new { message = "User is not enrolled in this subject." });
                }

                // ✅ Check if all previous lessons in the same topic are completed
                var previousLessons = await _learningProgressService.GetPreviousLessons(lesson.Id, lesson.TopicId);
                var completedLessons = await _learningProgressService.GetUserCompletedLessons(userId.Value, lesson.TopicId);

                if (previousLessons.Any(pl => !completedLessons.Contains(pl.Id)))
                {
                    return BadRequest(new { message = "You must complete previous lessons in this topic before proceeding." });
                }

                // ✅ Check if all lessons in previous topics are completed
                var previousTopics = await _learningProgressService.GetPreviousTopics(lesson.TopicId, lesson.Topic.SubjectId);
                foreach (var topic in previousTopics)
                {
                    var topicLessons = await _learningProgressService.GetLessonsByTopic(topic.Id);
                    if (topicLessons.Any(l => !completedLessons.Contains(l.Id)))
                    {
                        return BadRequest(new { message = "You must complete all lessons in previous topics before proceeding." });
                    }
                }

                // ✅ Check if the topic has a quiz
                var topicQuiz = await _learningProgressService.GetQuizByTopicId(lesson.TopicId);
                if (topicQuiz != null)
                {
                    // Check if all lessons in this topic are completed
                    var allLessonsInTopic = await _learningProgressService.GetLessonsByTopic(lesson.TopicId);
                    bool allLessonsCompleted = allLessonsInTopic.All(l => completedLessons.Contains(l.Id));

                    if (allLessonsCompleted)
                    {
                        // Check if the user has taken and passed the quiz
                        var UserQuizSubmission = await _learningProgressService.GetUserQuizSubmission(userId.Value, topicQuiz.Id);
                        if (UserQuizSubmission == null || UserQuizSubmission.Score < 60) // Pass mark = 60%
                        {
                            return BadRequest(new { message = "You must complete and pass the quiz for this topic before proceeding to the next topic." });
                        }
                    }
                }

                // ✅ Mark lesson as completed
                bool isMarked = await _learningProgressService.MarkLessonCompleted(lessonId, userId.Value);
                if (!isMarked)
                {
                    return NotFound(new { message = "Failed to mark lesson as completed." });
                }

                return Ok(new { message = "Lesson marked as completed!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking lesson {LessonId} as completed for user {UserId}", lessonId, userId);
                return StatusCode(500, new { message = "An error occurred while marking the lesson as completed.", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the user's progress for a given subject.
        /// </summary>
        [Authorize]
        [HttpGet("subject/{subjectId}")]
        public async Task<IActionResult> GetUserProgress([FromRoute] int subjectId)
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null) return Unauthorized(new { message = "User not authenticated." });

            _logger.LogInformation("Fetching progress for user {UserId} in subject {SubjectId}", userId, subjectId);

            try
            {
                var progress = await _learningProgressService.GetUserProgress(subjectId, userId.Value);

                if (progress == null || progress.Count == 0)
                {
                    return NotFound(new { message = "No progress found for this user and subject." });
                }

                return Ok(progress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching progress for user {UserId} in subject {SubjectId}", userId, subjectId);
                return StatusCode(500, new { message = "An error occurred while fetching progress." });
            }
        }

        /// <summary>
        /// Retrieves the last lesson the user visited.
        /// </summary>
        [Authorize]
        [HttpGet("user/last-lesson")]
        public async Task<IActionResult> GetLastLesson()
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null) return Unauthorized(new { message = "User not authenticated." });

            _logger.LogInformation("Fetching last visited lesson for user {UserId}", userId);

            try
            {
                var lastLesson = await _learningProgressService.GetLastVisitedLesson(userId.Value);

                if (lastLesson == null)
                {
                    return NotFound(new { message = "No lesson progress found." });
                }

                return Ok(new { lastLesson });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching last visited lesson for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving last lesson progress." });
            }
        }
    }
}
