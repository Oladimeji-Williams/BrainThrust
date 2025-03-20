namespace BrainThrust.src.Models.Dtos
{
    public class GetQuestionDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuestionText { get; set; }
        public int CorrectOptionId { get; set; } // ✅ Added this property
        public string CorrectOption { get; set; } // ✅ Correct answer text
        public List<string> Options { get; set; } = new List<string>(); // ✅ Prevent null errors
    }
}
