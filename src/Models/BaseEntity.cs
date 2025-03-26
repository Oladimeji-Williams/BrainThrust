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
        [Required]
        public DateTime? Modified { get; set; }
        [Required]
        public bool IsDeleted { get; set; } = false;
        [Required]
        public DateTime? DateDeleted { get; set; }
    }

}
