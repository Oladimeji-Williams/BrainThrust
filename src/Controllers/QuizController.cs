using BrainThrust.src.Models.Dtos;
using BrainThrust.src.Models.Entities;
using BrainThrust.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/quizzes")]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILearningProgressService _learningProgressService;
        private readonly UserService _userService;
        private readonly ILogger<QuizController> _logger;

        public QuizController(ApplicationDbContext context, ILearningProgressService learningProgressService, UserService userService, ILogger<QuizController> logger)
        {
            _context = context;
            _learningProgressService = learningProgressService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new quiz.
        /// </summary>
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizDto dto)
        {
            _logger.LogInformation("Received request to create a new quiz.");

            if (dto == null || string.IsNullOrWhiteSpace(dto.Title))
            {
                _logger.LogWarning("Quiz creation failed. Title is missing.");
                return BadRequest("Quiz title is required.");
            }

            bool topicExists = await _context.Topics.AnyAsync(m => m.Id == dto.TopicId);
            if (!topicExists)
            {
                _logger.LogWarning("Quiz creation failed. Topic {TopicId} does not exist.", dto.TopicId);
                return BadRequest("InvalId TopicId. Topic does not exist.");
            }

            var existingQuiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.TopicId == dto.TopicId);
            if (existingQuiz != null)
            {
                return BadRequest("A quiz already exists for this topic.");
            }

            var quiz = new Quiz
            {
                Title = dto.Title,
                TopicId = dto.TopicId
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Quiz {Id} created successfully.", quiz.Id);

            return CreatedAtAction(nameof(GetQuizByTopicId), new { TopicId = quiz.TopicId }, new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                TopicId = quiz.TopicId,
                Questions = new List<QuestionDto>()
            });
        }

        /// <summary>
        /// Retrieves a specific quiz by TopicId.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{TopicId}/details")]
        public async Task<ActionResult<QuizDto>> GetQuizByTopicId(int TopicId)
        {
            _logger.LogInformation("Received request to fetch quiz for Topic {TopicId}.", TopicId);

            var quiz = await _context.Quizzes
                .Where(q => q.TopicId == TopicId && !q.IsDeleted)
                .Include(q => q.Questions)
                .Select(q => new QuizDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    TopicId = q.TopicId,
                    Questions = q.Questions
                        .Where(qs => !qs.IsDeleted)
                        .Select(qs => new QuestionDto { Id = qs.Id, QuestionText = qs.QuestionText })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (quiz == null)
            {
                _logger.LogWarning("Quiz for Topic {TopicId} not found or has been deleted.", TopicId);
                return NotFound("Quiz not found or deleted.");
            }

            _logger.LogInformation("Quiz for Topic {TopicId} retrieved successfully.", TopicId);
            return Ok(quiz);
        }

        /// <summary>
        /// Updates an existing quiz by TopicId.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{TopicId}")]
        public async Task<IActionResult> UpdateQuizByTopicId(int TopicId, [FromBody] UpdateQuizDto dto)
        {
            _logger.LogInformation("Received request to update quiz for Topic {TopicId}.", TopicId);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.TopicId == TopicId);

            if (quiz == null)
            {
                _logger.LogWarning("Quiz update failed. No quiz found for Topic {TopicId}.", TopicId);
                return NotFound("Quiz not found.");
            }

            if (dto.IsDeleted.HasValue)
            {
                quiz.IsDeleted = dto.IsDeleted.Value;
                quiz.DateDeleted = dto.IsDeleted.Value ? DateTime.UtcNow : null;

                foreach (var question in quiz.Questions)
                {
                    question.IsDeleted = dto.IsDeleted.Value;
                    question.DateDeleted = dto.IsDeleted.Value ? DateTime.UtcNow : null;
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.Title))
                quiz.Title = dto.Title;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a quiz permanently by TopicId.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{TopicId}")]
        public async Task<IActionResult> DeleteQuizByTopicId(int TopicId)
        {
            _logger.LogInformation("Received request to permanently delete quiz for Topic {TopicId}.", TopicId);

            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.TopicId == TopicId);
            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();

            return Ok($"Quiz for Topic {TopicId} deleted.");
        }

        /// <summary>
        /// Retrieves a quiz with questions (excluding answers).
        /// </summary>
        [Authorize]
        [HttpGet("topic/{topicId}/quiz")]
        public async Task<ActionResult<QuizDto>> TakeQuizByTopic(int topicId)
        {
            var userId = _userService.GetLoggedInUserId(User);
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized quiz access attempt.");
                return Unauthorized("User not authenticated.");
            }

            var quiz = await _learningProgressService.GetQuizByTopicId(topicId);
            if (quiz == null)
            {
                _logger.LogWarning("No quiz found for Topic {TopicId}.", topicId);
                return NotFound("No quiz available for this topic.");
            }

            var topic = quiz.Topic;
            if (topic == null)
            {
                return NotFound("Quiz topic not found.");
            }

            // ✅ Check if user is enrolled in the subject
            bool isEnrolled = await _learningProgressService.IsUserEnrolledInSubject(userId.Value, topic.SubjectId);
            if (!isEnrolled)
            {
                _logger.LogWarning("User {UserId} tried to access quiz for topic {TopicId} without enrollment.", userId, topicId);
                return Unauthorized("You must be enrolled in this subject to take the quiz.");
            }

            // ✅ Check if all lessons are completed
            var totalLessons = await _learningProgressService.GetLessonsByTopic(topic.Id);
            var completedLessons = await _learningProgressService.GetUserCompletedLessons(userId.Value, topic.Id);

            if (completedLessons.Count < totalLessons.Count)
            {
                _logger.LogWarning("User {UserId} tried to take quiz for topic {TopicId} without completing all lessons.", userId, topicId);
                return Unauthorized("You must complete all lessons in this topic before taking the quiz.");
            }

            _logger.LogInformation("User {UserId} is taking quiz for topic {TopicId}.", userId, topicId);

            return Ok(new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                TopicId = quiz.TopicId,
                Questions = quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Options = q.Options.Select(o => new OptionDto
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
                }).ToList()

            });
        }

        /// <summary>
        /// Submits quiz answers and returns percentage-based score.
        /// </summary>
        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto submission)
        {
            if (submission?.Answers == null || !submission.Answers.Any())
            {
                return BadRequest("Invalid request. Quiz ID and answers are required.");
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(q => q.Id == submission.QuizId);

            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            var userId = _userService.GetLoggedInUserId(User);
            if (!userId.HasValue)
            {
                return Unauthorized("Invalid user ID.");
            }


            int totalQuestions = quiz.Questions.Count;
            int correctAnswers = 0;
            var userAttempts = new List<UserQuizSubmission>();
            var userQuizAttempt = new UserQuizAttempt
            {
                UserId = userId.Value,
                QuizId = submission.QuizId
            };

            _context.UserQuizAttempts.Add(userQuizAttempt);
            await _context.SaveChangesAsync(); // Save to get the generated Id

            foreach (var answer in submission.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                {
                    return BadRequest($"Question with ID {answer.QuestionId} is not part of this quiz.");
                }

                var selectedOption = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
                if (selectedOption == null)
                {
                    return BadRequest($"Selected option with ID {answer.SelectedOptionId} is invalid for question {answer.QuestionId}.");
                }

                var correctOption = question.GetCorrectOption();
                bool isCorrect = correctOption != null && correctOption.Id == answer.SelectedOptionId;

                if (isCorrect) 
                {
                    correctAnswers++;  // Now correctly counting the number of correct answers
                }

                userAttempts.Add(new UserQuizSubmission
                {
                    UserId = userId.Value,
                    QuizId = submission.QuizId,
                    QuestionId = question.Id,
                    SelectedOptionId = answer.SelectedOptionId,
                    Score = isCorrect ? 100 : 0,
                    Created = DateTime.UtcNow,
                    UserQuizAttemptId = userQuizAttempt.Id
                });
            }

            await _context.UserQuizSubmissions.AddRangeAsync(userAttempts);
            await _context.SaveChangesAsync();

            // Calculate total score percentage
            double totalScorePercentage = totalQuestions > 0 ? ((double)correctAnswers / totalQuestions) * 100 : 0;

            // Update UserQuizAttempt with results
            userQuizAttempt.CorrectAnswers = correctAnswers;
            userQuizAttempt.IncorrectAnswers = totalQuestions - correctAnswers;
            userQuizAttempt.TotalQuestions = totalQuestions;
            userQuizAttempt.TotalScore = totalScorePercentage;
            userQuizAttempt.IsPassed = totalScorePercentage >= 50; // Assuming 50% is the passing mark

            // Save the updated attempt
            _context.UserQuizAttempts.Update(userQuizAttempt);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Quiz submitted successfully",
                TotalScore = totalScorePercentage, // Total score as a percentage
                TotalQuestions = totalQuestions,
                CorrectAnswers = correctAnswers,
                IncorrectAnswers = totalQuestions - correctAnswers,
                Answers = userAttempts.Select(a => new
                {
                    a.QuestionId,
                    a.SelectedOptionId,
                    a.Score
                })
            });
        }

    }
}
