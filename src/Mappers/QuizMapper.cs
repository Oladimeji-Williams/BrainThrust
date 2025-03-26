using BrainThrust.src.Dtos.OptionDtos;
using BrainThrust.src.Dtos.QuestionDtos;
using BrainThrust.src.Dtos.QuizDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class QuizMapper
    {
        public static QuizDto ToQuizDto(Quiz quiz)
        {
            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                TopicId = quiz.TopicId,
                Questions = quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Options = q.Options.Select(o => new GetOptionDto
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
                }).ToList()
            };
        }

        public static QuizDto ToQuizDtoWithoutAnswers(Quiz quiz)
        {
            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                TopicId = quiz.TopicId,
                Questions = quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    QuestionText = q.QuestionText,
                    Options = q.Options.Select(o => new GetOptionDto
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
                }).ToList()
            };
        }
    }
}
