using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainThrust.src.Dtos.ReportDtos
{
    public class MostEnrolledSubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int Enrollments { get; set; }
    }
}

