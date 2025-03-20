namespace BrainThrust.src.Models.Dtos
{
    public class SubmitQuizDto
    {
        public int QuizId { get; set; }
        public List<UserAnswerDto>? Answers { get; set; }
    }
    
}
