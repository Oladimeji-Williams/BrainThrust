using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainThrust.src.Dtos.ReportDtos;
namespace BrainThrust.src.Dtos.ReportDtos
{
    public class EngagementStatsDto
    {
        public int ActiveLearners { get; set; }
        public List<MostEnrolledSubjectDto>? MostEnrolledSubjects { get; set; }
        public List<(int UserId, double TotalScore)>? TopScorers { get; set; }
    }
}