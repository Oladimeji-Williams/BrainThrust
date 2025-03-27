using BrainThrust.src.Dtos.ReportDtos;
using System.Security.Claims;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardData(ClaimsPrincipal user);
    }
}
