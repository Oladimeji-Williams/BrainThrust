using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models
{
    public abstract class BaseEntity
    {
        // public Guid Id { get; set; } = Guid.NewGuid();
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Modified { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DateDeleted { get; set; }
    }

}
