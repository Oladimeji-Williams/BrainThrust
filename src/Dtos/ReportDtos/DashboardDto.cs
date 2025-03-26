using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrainThrust.src.Dtos.ReportDtos
{
    public class DashboardDto
    {
        public UserStatsDto? Users { get; set; }
        public ContentStatsDto? Content { get; set; }
        public ProgressStatsDto? Progress { get; set; }
        public EngagementStatsDto? Engagement { get; set; }
        public QuizPerformanceDto? QuizPerformance { get; set; }
    }
}