using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainThrust.src.Dtos.ReportDtos
{
    public class QuizPerformanceDto
    {
        public List<RecentQuizAttemptDto> RecentQuizAttempts { get; set; }
        public List<TopScorerDto> TopScorers { get; set; }
        
    }
}