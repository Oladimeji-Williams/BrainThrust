namespace BrainThrust.src.Models.Dtos
{
    public class ResetPasswordDto
    {
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
