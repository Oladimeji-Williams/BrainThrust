using Xunit;
using BrainThrust.src.Services;
using BrainThrust.src.Models.Entities;
using Moq;
using BrainThrust.src.Data;

namespace BrainThrust.Tests.Services
{
    public class QuestionServiceTests
    {
        private readonly QuestionService _questionService;

        public QuestionServiceTests()
        {
            var dbContextMock = new Mock<ApplicationDbContext>(new Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>());
            _questionService = new QuestionService(dbContextMock.Object);
        }

        [Fact]
        public async Task AddQuestionAsync_ShouldThrowException_WhenLessThanThreeOptions()
        {
            var question = new Question
            {
                QuizId = 1, // Replace with actual Quiz ID
                QuestionText = "What is the capital of France?",
                Options = new List<Option>
                {
                    new Option { Id = 1, Text = "Paris" },
                    new Option { Id = 2, Text = "London" },
                    new Option { Id = 3, Text = "Berlin" }
                },
                CorrectOptionId = 1
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _questionService.AddQuestionAsync(question));
            Assert.Equal("A question must have at least 3 options.", exception.Message);
        }

        [Fact]
        public async Task AddQuestionAsync_ShouldThrowException_WhenMoreThanFiveOptions()
        {
            // Arrange
            var question = new Question
            {
                QuizId = 1, // Replace with actual Quiz ID
                QuestionText = "What is the capital of France?",
                Options = new List<Option>  // 6 options (too many)
                {
                    new Option { Id = 1, Text = "Option 1" },
                    new Option { Id = 2, Text = "Option 2" },
                    new Option { Id = 3, Text = "Option 3" },
                    new Option { Id = 4, Text = "Option 4" },
                    new Option { Id = 5, Text = "Option 5" },
                    new Option { Id = 6, Text = "Option 6" }
                },
                CorrectOptionId = 1
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _questionService.AddQuestionAsync(question));
            Assert.Equal("A question can have at most 5 options.", exception.Message);
        }

        [Fact]
        public async Task AddQuestionAsync_ShouldThrowException_WhenCorrectOptionIsInvalid()
        {
            // Arrange
            var question = new Question
            {
                QuizId = 1, // Replace with actual Quiz ID
                QuestionText = "What is the capital of France?",
                Options = new List<Option>
                {
                    new Option { Id = 1, Text = "Option 1" },
                    new Option { Id = 2, Text = "Option 2" },
                    new Option { Id = 3, Text = "Option 3" }
                },
                CorrectOptionId = 99 // Not in options
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _questionService.AddQuestionAsync(question));
            Assert.Equal("The correct option must be one of the question's options.", exception.Message);
        }

        [Fact]
        public async Task AddQuestionAsync_ShouldPass_WhenValidQuestionProvided()
        {
            // Arrange
            var question = new Question
            {
                QuizId = 1, // Replace with actual Quiz ID
                QuestionText = "What is the capital of France?",
                Options = new List<Option>
                {
                    new Option { Id = 1, Text = "Option 1" },
                    new Option { Id = 2, Text = "Option 2" },
                    new Option { Id = 3, Text = "Option 3" }
                },
                CorrectOptionId = 2
            };

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => _questionService.AddQuestionAsync(question));
            Assert.Null(exception); // No exception should be thrown
        }
    }

}
