using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainThrust.src.Dtos.ReportDtos
{
    public class RecentQuizAttemptDto
    {
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int TotalScore { get; set; }
        public bool IsPassed { get; set; }
        public DateTime Created { get; set; }
    }
}