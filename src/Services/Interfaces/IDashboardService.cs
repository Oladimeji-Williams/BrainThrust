using BrainThrust.src.Dtos.ReportDtos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardData(ClaimsPrincipal user);
    }
}
