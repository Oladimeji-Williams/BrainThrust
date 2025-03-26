using BrainThrust.src.Dtos.OptionDtos;

namespace BrainThrust.src.Dtos.QuestionDtos
{
    public class GetQuestionDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string? QuestionText { get; set; }
        public int CorrectOptionId { get; set; }
        public string? CorrectOption { get; set; }
        public List<GetOptionDto> Options { get; set; } = new List<GetOptionDto>();
    }
}
