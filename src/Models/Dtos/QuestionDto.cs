namespace BrainThrust.src.Models.Dtos
{

    public class QuestionDto
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<OptionDto> Options { get; set; } = new();
    }
}
