using BrainThrust.src.Dtos.UserQuizSubmissionDtos;

namespace BrainThrust.src.Dtos.SubmissionDtos
{
    public class SubmitQuizDto
    {
        public int QuizId { get; set; }
        public List<UserAnswerDto> Submissions { get; set; } = new List<UserAnswerDto>();
    }
}
