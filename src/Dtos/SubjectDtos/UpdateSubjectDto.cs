namespace BrainThrust.src.Dtos.SubjectDtos
{
    public class UpdateSubjectDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool? IsDeleted { get; set; } // Ensure this exists to support restoring
    }
}
