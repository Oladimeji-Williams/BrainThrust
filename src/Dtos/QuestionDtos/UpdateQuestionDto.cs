using BrainThrust.src.Dtos.OptionDtos;

namespace BrainThrust.src.Dtos.QuestionDtos
{
    public class UpdateQuestionDto
    {
        public string? QuestionText { get; set; }
        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
        public string? CorrectOption { get; set; }
    }

}