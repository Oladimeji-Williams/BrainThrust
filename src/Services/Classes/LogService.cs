using BrainThrust.src.Dtos.LogDtos;
using BrainThrust.src.Services.Interfaces;

namespace BrainThrust.src.Services.Classes
{
    public class LogService : ILogService
    {
        private readonly string _logDirectory;
        private readonly ILogger<LogService> _logger;

        public LogService(ILogger<LogService> logger)
        {
            _logger = logger;
            _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            if (!Directory.Exists(_logDirectory))
            {
                _logger.LogWarning("Log directory does not exist: {Path}", _logDirectory);
                Directory.CreateDirectory(_logDirectory); // Ensure directory exists
            }
        }

        /// <summary>
        /// Get a list of available log files.
        /// </summary>
        public List<LogFileDto> GetLogFiles()
        {
            if (!Directory.Exists(_logDirectory))
            {
                return new List<LogFileDto>();
            }

            var files = Directory.GetFiles(_logDirectory)
                .Select(filePath => new FileInfo(filePath))
                .Select(fileInfo => new LogFileDto
                {
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                    FileSize = fileInfo.Length,
                    CreatedOn = fileInfo.CreationTime
                })
                .ToList();

            return files;
        }

        /// <summary>
        /// Retrieve a file stream for downloading a log file.
        /// </summary>
        public async Task<FileStream?> GetLogFileStreamAsync(string filename)
        {
            string filePath = Path.Combine(_logDirectory, filename);

            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                // Open file in Read mode with FileShare.ReadWrite to allow simultaneous access
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return stream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing log file: {Filename}", filename);
                return null;
            }
        }
    }
}
