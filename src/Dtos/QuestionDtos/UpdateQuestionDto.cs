using BrainThrust.src.Dtos.OptionDtos;

namespace BrainThrust.src.Dtos.QuestionDtos
{
    public class UpdateQuestionDto
    {
        public string QuestionText { get; set; } = string.Empty;  // Question text
        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();  // Options for the question

        public string? CorrectOption { get; set; }  // ✅ Added this if you need the text of the correct option
    }

}