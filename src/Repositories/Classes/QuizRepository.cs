using BrainThrust.src.Dtos.QuizDtos;
using BrainThrust.src.Dtos.SubmissionDtos;
using BrainThrust.src.Dtos.UserQuizSubmissionDtos;
using BrainThrust.src.Models.Entities;
using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Repositories.Interfaces;

namespace BrainThrust.src.Repositories.Classes
{
    public class QuizRepository : IQuizRepository
    {
        private readonly ApplicationDbContext _context;

        public QuizRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Quiz> GetQuizByTopicId(int topicId)
        {
            return await _context.Quizzes.FirstOrDefaultAsync(q => q.TopicId == topicId);
        }

        public async Task<Quiz> CreateQuiz(Quiz quiz)
        {
            var topicExists = await _context.Topics.AnyAsync(t => t.Id == quiz.TopicId);
            if (!topicExists)
            {
                throw new Exception("Invalid TopicId. The topic does not exist.");
            }

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task<bool> UpdateQuiz(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteQuiz(int topicId)
        {
            var quiz = await _context.Quizzes.FirstOrDefaultAsync(q => q.TopicId == topicId);
            if (quiz == null) return false;

            _context.Quizzes.Remove(quiz);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<SubmitQuizResponseDto> SubmitQuiz(SubmitQuizDto submitQuizDto, int userId)
        {
            if (submitQuizDto == null)
            {
                throw new ArgumentNullException(nameof(submitQuizDto), "Quiz submission data is required.");
            }

            // Validate if the quiz exists
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == submitQuizDto.QuizId);

            if (quiz == null)
            {
                throw new ArgumentException($"Quiz with ID {submitQuizDto.QuizId} does not exist. Please submit a valid quiz.");
            }

            // Total number of questions in the quiz (not just answered ones)
            int totalQuestions = quiz.Questions.Count;

            if (totalQuestions == 0)
            {
                throw new InvalidOperationException("This quiz has no questions available.");
            }

            // Ensure at least one submission
            if (submitQuizDto.Submissions == null || !submitQuizDto.Submissions.Any())
            {
                throw new ArgumentException("Your submission does not contain any answers. Please answer at least one question before submitting.");
            }

            var attempt = new UserQuizAttempt
            {
                UserId = userId,
                QuizId = submitQuizDto.QuizId,
                TotalQuestions = totalQuestions,
                CorrectAnswers = 0,
                IncorrectAnswers = 0,
                TotalScore = 0,
                IsPassed = false
            };

            _context.UserQuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            var answers = new List<UserAnswerDto>();

            foreach (var sub in submitQuizDto.Submissions)
            {
                var question = await _context.Questions.FindAsync(sub.QuestionId);
                if (question == null)
                {
                    throw new ArgumentException($"Question with ID {sub.QuestionId} does not exist. Please check your submission and try again.");
                }

                var optionExists = await _context.Options.AnyAsync(o => o.Id == sub.SelectedOptionId && o.QuestionId == sub.QuestionId);
                if (!optionExists)
                {
                    throw new ArgumentException($"Selected option with ID {sub.SelectedOptionId} does not exist for Question ID {sub.QuestionId}. Please select a valid option.");
                }

                bool isCorrect = sub.SelectedOptionId == question.CorrectOptionId;
                var score = isCorrect ? question.Score : 0;

                var userSubmission = new UserQuizSubmission
                {
                    UserId = userId,
                    QuizId = submitQuizDto.QuizId,
                    QuestionId = sub.QuestionId,
                    SelectedOptionId = sub.SelectedOptionId,
                    UserQuizAttemptId = attempt.Id
                };

                _context.UserQuizSubmissions.Add(userSubmission);

                if (isCorrect)
                    attempt.CorrectAnswers++;
                else
                    attempt.IncorrectAnswers++;

                answers.Add(new UserAnswerDto
                {
                    QuestionId = sub.QuestionId,
                    SelectedOptionId = sub.SelectedOptionId
                });
            }

            // Calculate score based on total quiz questions, not only answered questions
            attempt.TotalScore = (attempt.CorrectAnswers / (double)totalQuestions) * 100;
            attempt.IsPassed = attempt.TotalScore >= 60;

            await _context.SaveChangesAsync();

            return new SubmitQuizResponseDto
            {
                TotalQuestions = totalQuestions,
                CorrectAnswers = attempt.CorrectAnswers,
                IncorrectAnswers = attempt.IncorrectAnswers,
                TotalScore = (int)attempt.TotalScore,
                IsPassed = attempt.IsPassed,
                Message = attempt.IsPassed ? "Congratulations! You passed the quiz." : "You did not pass the quiz. Keep practicing!",
                Answers = answers
            };
        }

    }
}
