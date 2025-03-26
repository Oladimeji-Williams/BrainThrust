namespace BrainThrust.src.Dtos.EnrollmentDtos
{
    public class GetEnrollmentDto
    {
        public int UserId { get; set; }
        public string? UserFirstName { get; set; }  // Add this line
        public int SubjectId { get; set; }
        public string SubjectTitle { get; set; } = string.Empty;
    }
}
