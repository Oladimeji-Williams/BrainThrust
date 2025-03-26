using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainThrust.src.Dtos.ReportDtos
{
    public class ProgressStatsDto
    {
        public int CompletedSubjects { get; set; }
        public int CompletedTopics { get; set; }
        public int CompletedLessons { get; set; }
        public int CompletedQuizzes { get; set; }
    }
}