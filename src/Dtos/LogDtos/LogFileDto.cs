namespace BrainThrust.src.Dtos.LogDtos
{
    public class LogFileDto
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
