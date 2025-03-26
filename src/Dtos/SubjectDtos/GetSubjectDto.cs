using BrainThrust.src.Dtos.TopicDtos;

namespace BrainThrust.src.Dtos.SubjectDtos
{
    public class GetSubjectDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
