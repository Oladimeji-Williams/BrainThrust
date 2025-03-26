using BrainThrust.src.Dtos.OptionDtos;

namespace BrainThrust.src.Dtos.QuestionDtos
{

    public class QuestionDto
    {
        public int Id { get; set; }
        public string? QuestionText { get; set; }
        public List<GetOptionDto> Options { get; set; } = new List<GetOptionDto>();
    }
}
