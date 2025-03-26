using BrainThrust.src.Dtos.OptionDtos;

namespace BrainThrust.src.Dtos.QuestionDtos
{
    public class CreateQuestionDto
    {
        public string? QuestionText { get; set; }
        public List<CreateOptionDto>? Options { get; set; }
        public string? CorrectOption { get; set; }
    }

}
