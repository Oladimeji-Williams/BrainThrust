namespace BrainThrust.src.Models.Dtos
{
    public class RestoreEnrollmentDto
    {
        public int UserId { get; set; }
        public int SubjectId { get; set; }
        public string? Message { get; set; }
    }
}
