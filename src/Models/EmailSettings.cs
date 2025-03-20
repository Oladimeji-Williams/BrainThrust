
using System.ComponentModel.DataAnnotations;

namespace BrainThrust.src.Models
{
    public class EmailSettings
    {
        [Required]
        public required string SmtpServer { get; set; }
        [Required]
        public required int Port { get; set; }
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required string FromEmail { get; set; }
        [Required]
        public required bool EnableSsl { get; set; }
    }
}
