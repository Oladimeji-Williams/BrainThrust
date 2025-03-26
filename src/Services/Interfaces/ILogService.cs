using BrainThrust.src.Dtos.LogDtos;

namespace BrainThrust.src.Services.Interfaces
{
    public interface ILogService
    {
        List<LogFileDto> GetLogFiles();
        Task<FileStream?> GetLogFileStreamAsync(string filename);
    }
}
