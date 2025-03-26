using BrainThrust.src.Dtos.QuestionDtos;
using BrainThrust.src.Dtos.OptionDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class QuestionMapper
    {
        // Maps Question entity to GetQuestionDto
        public static GetQuestionDto ToQuestionDto(this Question question)
        {
            return new GetQuestionDto
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                QuizId = question.QuizId,
                Options = question.Options.Select(o => new GetOptionDto
                {
                    Id = o.Id,
                    Text = o.Text
                }).ToList(),
                CorrectOptionId = question.CorrectOptionId,
                CorrectOption = question.Options.FirstOrDefault(o => o.Id == question.CorrectOptionId)?.Text
            };
        }

        // Maps CreateQuestionDto to Question entity
        public static Question ToQuestion(this CreateQuestionDto createQuestionDto, int quizId)
        {
            var question = new Question
            {
                QuestionText = createQuestionDto.QuestionText,
                QuizId = quizId
            };

            var options = createQuestionDto.Options
                .Select(o => new Option { Text = o.Text })
                .ToList();

            question.Options = options;

            var correctOption = options.FirstOrDefault(o => o.Text == createQuestionDto.CorrectOption);
            if (correctOption != null)
            {
                question.CorrectOptionId = correctOption.Id;
            }

            return question;
        }

        // Updates an existing Question entity from UpdateQuestionDto
        public static void UpdateQuestionFromDto(this Question question, UpdateQuestionDto updateQuestionDto)
        {
            if (!string.IsNullOrEmpty(updateQuestionDto.QuestionText))
            {
                question.QuestionText = updateQuestionDto.QuestionText;
            }

            if (updateQuestionDto.Options != null && updateQuestionDto.Options.Any())
            {
                question.Options.Clear();
                foreach (var optionDto in updateQuestionDto.Options)
                {
                    question.Options.Add(new Option { Text = optionDto.Text });
                }
            }

            if (!string.IsNullOrEmpty(updateQuestionDto.CorrectOption))
            {
                var correctOption = question.Options.FirstOrDefault(o => o.Text == updateQuestionDto.CorrectOption);
                if (correctOption != null)
                {
                    question.CorrectOptionId = correctOption.Id;
                }
            }
        }
    }
}
