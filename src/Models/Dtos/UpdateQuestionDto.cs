namespace BrainThrust.src.Models.Dtos
{
    public class UpdateQuestionDto
    {
        public int QuizId { get; set; }  // Required to associate with a quiz
        public string QuestionText { get; set; } = string.Empty;  // Question text

        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();  // Options for the question

        public int? CorrectOptionId { get; set; }  // The Id of the correct option
        public int? CorrectOptionIndex { get; set; }  // Add this if it makes sense for updates

    }

}