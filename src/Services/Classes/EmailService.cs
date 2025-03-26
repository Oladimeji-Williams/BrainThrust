using System.Net;
using System.Net.Mail;
using BrainThrust.src.Models;
using BrainThrust.src.Services.Interfaces;

namespace BrainThrust.src.Services.Classes
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(EmailSettingsProvider settingsProvider, ILogger<EmailService> logger)
        {
            _logger = logger;
            _emailSettings = settingsProvider.LoadEmailSettings();
        }
        public string FromEmail => _emailSettings.FromEmail;
        
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(_emailSettings.FromEmail))
            {
                throw new ArgumentException("Sender email address cannot be null or empty", nameof(_emailSettings.FromEmail));
            }

            try
            {
                using (var client = new System.Net.Mail.SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                {
                    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    client.EnableSsl = _emailSettings.EnableSsl;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.FromEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("📧 Email sent to {ToEmail} successfully.", toEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Failed to send email to {ToEmail}", toEmail);
                throw;
            }
        }

        public async Task SendPasswordResetEmail(string email, string resetLink)
        {
            string subject = "Password Reset Request";
            string body = $"Click <a href='{resetLink}'>here</a> to reset your password.";
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetConfirmationAsync(string email)
        {
            string subject = "Password Reset Successful";
            string body = "Your password has been reset successfully.";
            await SendEmailAsync(email, subject, body);
        }
    }
}
