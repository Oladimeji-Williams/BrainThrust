using BrainThrust.src.Models;
using Microsoft.Extensions.Logging;
using System;
namespace BrainThrust.src.Services
{
    public class EmailSettingsProvider
    {
        private readonly ILogger<EmailSettingsProvider> _logger;

        public EmailSettingsProvider(ILogger<EmailSettingsProvider> logger)
        {
            _logger = logger;
        }

        public EmailSettings LoadEmailSettings()
        {
            try
            {
                return new EmailSettings
                {
                    SmtpServer = ValidateEnvironmentVariable("SMTP_SERVER"),
                    Port = int.Parse(Environment.GetEnvironmentVariable("EMAIL_PORT") ?? "587"),
                    Username = ValidateEnvironmentVariable("EMAIL_USERNAME"),
                    Password = ValidateEnvironmentVariable("EMAIL_PASSWORD"),
                    FromEmail = ValidateEnvironmentVariable("FROM_EMAIL"),  // 🛑 Ensure this is not null
                    EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("EnableSsl"), out bool enableSsl) && enableSsl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Failed to load email settings.");
                throw;
            }
        }

        private string ValidateEnvironmentVariable(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Environment variable '{key}' is missing or empty.");
            }
            return value;
        }
    }

}