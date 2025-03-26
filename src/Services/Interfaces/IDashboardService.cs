using BrainThrust.src.Dtos.ReportDtos;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardData();
    }
}
