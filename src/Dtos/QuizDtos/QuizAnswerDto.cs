namespace BrainThrust.src.Dtos.QuizDtos
{
    public class QuizAnswerDto
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; } // ✅ This might be the correct property
    }
}
