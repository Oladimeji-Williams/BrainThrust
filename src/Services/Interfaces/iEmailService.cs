namespace BrainThrust.src.Services.Interfaces
{
    public interface IEmailService
    {
        string FromEmail { get; }


        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPasswordResetEmail(string toEmail, string token);
        Task SendPasswordResetConfirmationAsync(string toEmail);
    }
}
