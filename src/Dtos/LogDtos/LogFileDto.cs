namespace BrainThrust.src.Dtos.LogDtos
{
    public class LogFileDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; } // Size in bytes
        public DateTime CreatedOn { get; set; }
    }
}
