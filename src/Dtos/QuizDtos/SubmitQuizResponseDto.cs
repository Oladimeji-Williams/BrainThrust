using BrainThrust.src.Dtos.UserQuizSubmissionDtos;

namespace BrainThrust.src.Dtos.QuizDtos
{
    public class SubmitQuizResponseDto
    {
        public string Message { get; set; }
        public int TotalScore { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public bool IsPassed { get; set; }
        public List<UserAnswerDto> Answers { get; set; }  // <-- Add this if missing
    }
}
