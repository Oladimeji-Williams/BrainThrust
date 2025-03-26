using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainThrust.src.Dtos.ReportDtos
{
    public class ContentStatsDto
    {
        public int TotalSubjects { get; set; }
        public int TotalTopics { get; set; }
        public int TotalLessons { get; set; }
        public int TotalQuizzes { get; set; }
        public List<(int SubjectId, int Enrollments)> MostEnrolledSubjects { get; set; }

    }
}