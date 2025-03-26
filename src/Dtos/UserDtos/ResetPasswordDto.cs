namespace BrainThrust.src.Dtos.UserDtos
{
    public class ResetPasswordDto
    {
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
