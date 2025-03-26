
using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models
{
    public class EmailSettings
    {
        [Required]
        public string? SmtpServer { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? FromEmail { get; set; }
        [Required]
        public bool EnableSsl { get; set; }
    }
}
