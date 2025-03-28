using BrainThrust.src.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BrainThrust_API.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Fetches overall statistics for the dashboard.
        /// </summary>
        /// <returns>DashboardDTO with user, content, progress, engagement, and quiz stats.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var dashboardData = await _dashboardService.GetDashboardData(User);
            return Ok(dashboardData);
        }
    }
}


