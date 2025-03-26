using BrainThrust.src.Dtos.QuizDtos;
using BrainThrust.src.Dtos.SubmissionDtos;
using BrainThrust.src.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IUserService _userService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(IQuizService quizService, IUserService userService, ILogger<QuizController> logger)
        {
            _quizService = quizService ?? throw new ArgumentNullException(nameof(quizService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a new quiz.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto createQuizDto)
        {
            if (createQuizDto == null)
            {
                _logger.LogWarning("CreateQuiz: Received null CreateQuizDto.");
                return BadRequest("Invalid quiz data.");
            }

            if (!await _quizService.TopicExists(createQuizDto.TopicId))
            {
                _logger.LogWarning("CreateQuiz: Invalid TopicId {TopicId}.", createQuizDto.TopicId);
                return BadRequest("The provided TopicId does not exist.");
            }

            try
            {
                var quiz = await _quizService.CreateQuiz(createQuizDto).ConfigureAwait(false);
                _logger.LogInformation("CreateQuiz: Quiz created successfully for TopicId {TopicId}.", quiz.TopicId);
                return CreatedAtAction(nameof(GetQuizByTopicId), new { topicId = quiz.TopicId }, quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateQuiz: Error creating quiz for TopicId {TopicId}.", createQuizDto.TopicId);
                return StatusCode(500, "An error occurred while creating the quiz.");
            }
        }

        /// <summary>
        /// Gets quiz details by topic ID.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{topicId}/details")]
        public async Task<IActionResult> GetQuizByTopicId(int topicId)
        {
            try
            {
                var quiz = await _quizService.GetQuizByTopicId(topicId).ConfigureAwait(false);
                if (quiz == null)
                {
                    _logger.LogWarning("GetQuizByTopicId: Quiz not found for TopicId {TopicId}.", topicId);
                    return NotFound("Quiz not found.");
                }

                _logger.LogInformation("GetQuizByTopicId: Retrieved quiz for TopicId {TopicId}.", topicId);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetQuizByTopicId: Error retrieving quiz for TopicId {TopicId}.", topicId);
                return StatusCode(500, "An error occurred while retrieving the quiz.");
            }
        }

        /// <summary>
        /// Updates a quiz by topic ID.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{topicId}")]
        public async Task<IActionResult> UpdateQuizByTopicId(int topicId, [FromBody] UpdateQuizDto updateQuizDto)
        {
            if (updateQuizDto == null)
            {
                _logger.LogWarning("UpdateQuizByTopicId: Received null UpdateQuizDto for TopicId {TopicId}.", topicId);
                return BadRequest("Invalid update data.");
            }

            try
            {
                var success = await _quizService.UpdateQuizByTopicId(topicId, updateQuizDto).ConfigureAwait(false);
                if (!success)
                {
                    _logger.LogWarning("UpdateQuizByTopicId: Quiz not found for TopicId {TopicId}.", topicId);
                    return NotFound("Quiz not found.");
                }

                _logger.LogInformation("UpdateQuizByTopicId: Quiz updated successfully for TopicId {TopicId}.", topicId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateQuizByTopicId: Error updating quiz for TopicId {TopicId}.", topicId);
                return StatusCode(500, "An error occurred while updating the quiz.");
            }
        }

        /// <summary>
        /// Deletes a quiz by topic ID.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{topicId}")]
        public async Task<IActionResult> DeleteQuizByTopicId(int topicId)
        {
            try
            {
                var success = await _quizService.DeleteQuizByTopicId(topicId).ConfigureAwait(false);
                if (!success)
                {
                    _logger.LogWarning("DeleteQuizByTopicId: Quiz not found for TopicId {TopicId}.", topicId);
                    return NotFound("Quiz not found.");
                }

                _logger.LogInformation("DeleteQuizByTopicId: Quiz deleted successfully for TopicId {TopicId}.", topicId);
                return Ok(new { message = "Quiz deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteQuizByTopicId: Error deleting quiz for TopicId {TopicId}.", topicId);
                return StatusCode(500, "An error occurred while deleting the quiz.");
            }
        }

        /// <summary>
        /// Retrieves a quiz for the user to take.
        /// </summary>
        [Authorize]
        [HttpGet("topic/{topicId}/quiz")]
        public async Task<IActionResult> TakeQuizByTopic(int topicId)
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (!userId.HasValue)
            {
                _logger.LogWarning("TakeQuizByTopic: Unauthorized access attempt.");
                return Unauthorized("User not found.");
            }

            try
            {
                var quiz = await _quizService.TakeQuizByTopic(topicId, userId.Value).ConfigureAwait(false);
                if (quiz == null)
                {
                    _logger.LogWarning("TakeQuizByTopic: Quiz not found for TopicId {TopicId}, User {UserId}.", topicId, userId);
                    return NotFound("Quiz not found.");
                }

                _logger.LogInformation("TakeQuizByTopic: Quiz retrieved successfully for TopicId {TopicId}, User {UserId}.", topicId, userId);
                return Ok(quiz);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TakeQuizByTopic: Error retrieving quiz for TopicId {TopicId}, User {UserId}.", topicId, userId);
                return StatusCode(500, "An error occurred while retrieving the quiz.");
            }
        }

        /// <summary>
        /// Submits a quiz attempt.
        /// </summary>
        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto submitQuizDto)
        {
            if (submitQuizDto == null)
            {
                _logger.LogWarning("SubmitQuiz: Received null SubmitQuizDto.");
                return BadRequest(new { message = "Invalid submission data. Please provide valid quiz answers." });
            }

            var userId = _userService.GetLoggedInUserId(User);
            if (!userId.HasValue)
            {
                _logger.LogWarning("SubmitQuiz: Unauthorized access attempt.");
                return Unauthorized(new { message = "User authentication failed. Please log in again." });
            }

            try
            {
                var result = await _quizService.SubmitQuiz(submitQuizDto, userId.Value).ConfigureAwait(false);
                _logger.LogInformation("SubmitQuiz: Quiz submitted successfully by User {UserId}.", userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "SubmitQuiz: Validation error for User {UserId}.", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubmitQuiz: Unexpected error submitting quiz for User {UserId}.", userId);
                return StatusCode(500, new { message = "An error occurred while submitting the quiz. Please try again later." });
            }
        }
    }
}
