using System;

namespace BrainThrust.src.Models.Dtos
{
    public class SubjectProgressDto
    {
        public int SubjectId { get; set; }
        public int UserId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
    }
}
