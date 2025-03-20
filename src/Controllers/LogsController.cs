using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogger<LogsController> logger)
        {
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
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            if (!Directory.Exists(logDirectory))
            {
                _logger.LogWarning("Log directory not found: {Path}", logDirectory);
                return NotFound(new { message = "Log directory does not exist." });
            }

            var files = Directory.GetFiles(logDirectory);
            var fileNames = Array.ConvertAll(files, Path.GetFileName);

            return Ok(fileNames);
        }

        /// <summary>
        /// Download a specific log file.
        /// Example: GET /api/logs/log-2025-03-10.txt
        /// </summary>
        [HttpGet("{filename}")]
        public IActionResult DownloadLogFile(string filename)
        {
            string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            string filePath = Path.Combine(logDirectory, filename);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Log file not found." });
            }

            try
            {
                // Open file in Read mode with FileShare.ReadWrite to allow simultaneous access
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                _logger.LogInformation("Log file {Filename} was downloaded.", filename);

                return File(stream, "text/plain", filename);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while downloading log file {Filename}", filename);
                return StatusCode(500, new { message = "Error downloading log file." });
            }
        }
    }
}
