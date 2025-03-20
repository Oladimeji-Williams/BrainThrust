namespace BrainThrust.src.Services
{
    public interface IEmailService  // Change from IMailService to IEmailService
    {
        string FromEmail { get; }  // ✅ Read-only property


        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendPasswordResetEmail(string toEmail, string token);
        Task SendPasswordResetConfirmationAsync(string toEmail);
    }
}
