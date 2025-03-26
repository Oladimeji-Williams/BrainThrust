using BrainThrust.src.Dtos.QuizDtos;
using BrainThrust.src.Dtos.SubmissionDtos;
using BrainThrust.src.Mappers;
using BrainThrust.src.Models.Entities;
using BrainThrust.src.Repositories.Interfaces;
using BrainThrust.src.Services.Interfaces;


namespace BrainThrust.src.Services.Classes
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ITopicRepository _topicRepository;
        private readonly ILearningProgressService _learningProgressService;
        private readonly ILogger<QuizService> _logger;

        public QuizService(
            IQuizRepository quizRepository, 
            ITopicRepository topicRepository,  
            ILearningProgressService learningProgressService, 
            ILogger<QuizService> logger)
        {
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
            _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
            _learningProgressService = learningProgressService ?? throw new ArgumentNullException(nameof(learningProgressService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> TopicExists(int topicId)
        {
            return await _topicRepository.TopicExists(topicId);
        }

        public async Task<QuizDto> CreateQuiz(CreateQuizDto createQuizDto)
        {
            if (createQuizDto == null) throw new ArgumentNullException(nameof(createQuizDto));

            _logger.LogInformation("Creating a new quiz for topic ID {TopicId}", createQuizDto.TopicId);

            if (!await TopicExists(createQuizDto.TopicId))
            {
                _logger.LogWarning("Invalid TopicId {TopicId} provided", createQuizDto.TopicId);
                throw new ArgumentException("The provided TopicId does not exist.");
            }

            var existingQuiz = await _quizRepository.GetQuizByTopicId(createQuizDto.TopicId);
            if (existingQuiz != null)
            {
                _logger.LogWarning("A quiz already exists for topic ID {TopicId}", createQuizDto.TopicId);
                throw new InvalidOperationException("A quiz already exists for this topic.");
            }

            var quiz = new Quiz
            {
                Title = createQuizDto.Title,
                TopicId = createQuizDto.TopicId
            };

            var createdQuiz = await _quizRepository.CreateQuiz(quiz);
            return QuizMapper.ToQuizDto(createdQuiz);
        }

        public async Task<QuizDto?> GetQuizByTopicId(int topicId)
        {
            _logger.LogInformation("Fetching quiz for topic ID {TopicId}", topicId);

            var quiz = await _quizRepository.GetQuizByTopicId(topicId);
            return quiz == null ? null : QuizMapper.ToQuizDto(quiz);
        }

        public async Task<bool> UpdateQuizByTopicId(int topicId, UpdateQuizDto updateQuizDto)
        {
            if (updateQuizDto == null) throw new ArgumentNullException(nameof(updateQuizDto));

            _logger.LogInformation("Updating quiz for topic ID {TopicId}", topicId);

            var quiz = await _quizRepository.GetQuizByTopicId(topicId);
            if (quiz == null) return false;

            quiz.Title = !string.IsNullOrWhiteSpace(updateQuizDto.Title) ? updateQuizDto.Title : quiz.Title;

            return await _quizRepository.UpdateQuiz(quiz);
        }

        public async Task<bool> DeleteQuizByTopicId(int topicId)
        {
            _logger.LogInformation("Deleting quiz for topic ID {TopicId}", topicId);
            return await _quizRepository.DeleteQuiz(topicId);
        }

        public async Task<QuizDto?> TakeQuizByTopic(int topicId, int userId)
        {
            _logger.LogInformation("User {UserId} is taking quiz for topic ID {TopicId}", userId, topicId);

            var quiz = await _quizRepository.GetQuizByTopicId(topicId);
            return quiz == null ? null : QuizMapper.ToQuizDtoWithoutAnswers(quiz);
        }


        public async Task<SubmitQuizResponseDto> SubmitQuiz(SubmitQuizDto submitQuizDto, int userId)
        {
            if (submitQuizDto == null) throw new ArgumentNullException(nameof(submitQuizDto));

            _logger.LogInformation("User {UserId} submitted quiz for quiz ID {QuizId}", userId, submitQuizDto.QuizId);

            var result = await _quizRepository.SubmitQuiz(submitQuizDto, userId);

            return new SubmitQuizResponseDto
            {
                Message = result.IsPassed ? "Congratulations! You passed the quiz." : "You did not pass the quiz. Keep practicing!",
                TotalScore = result.TotalScore,
                TotalQuestions = result.TotalQuestions,
                CorrectAnswers = result.CorrectAnswers,
                IncorrectAnswers = result.IncorrectAnswers,
                IsPassed = result.IsPassed,
                Answers = result.Answers
            };
        }

    }
}
