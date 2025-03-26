using BrainThrust.src.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogService logService, ILogger<LogsController> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        /// <summary>
        /// Get a list of available log files.
        /// Example: GET /api/logs
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ListLogFiles()
        {
            var logFiles = _logService.GetLogFiles();
            return Ok(logFiles);
        }

        /// <summary>
        /// Download a specific log file.
        /// Example: GET /api/logs/log-2025-03-10.txt
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{filename}")]
        public async Task<IActionResult> DownloadLogFile(string filename)
        {
            var stream = await _logService.GetLogFileStreamAsync(filename);

            if (stream == null)
            {
                _logger.LogWarning("Log file {Filename} not found.", filename);
                return NotFound(new { message = "Log file not found." });
            }

            _logger.LogInformation("Log file {Filename} was downloaded.", filename);
            return File(stream, "text/plain", filename);
        }
    }
}
