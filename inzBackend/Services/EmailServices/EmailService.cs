using System.Net;
using System.Net.Mail;

namespace inzBackend.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var host = _configuration["SmtpSettings:Host"] ?? _configuration["SMTP_HOST"] ?? "smtp.gmail.com";
            var portString = _configuration["SmtpSettings:Port"] ?? _configuration["SMTP_PORT"] ?? "587";
            var enableSslString = _configuration["SmtpSettings:EnableSsl"] ?? _configuration["SMTP_ENABLE_SSL"] ?? "true";
            var username = _configuration["SmtpSettings:Username"] ?? _configuration["SMTP_USERNAME"];
            var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? _configuration["SMTP_SENDER_EMAIL"];
            if (string.IsNullOrEmpty(senderEmail))
            {
                senderEmail = username;
            }
            var senderName = _configuration["SmtpSettings:SenderName"] ?? _configuration["SMTP_SENDER_NAME"] ?? "Gmitrzak English Academy";
            
            var rawPassword = _configuration["SmtpSettings:Password"] 
                ?? _configuration["SMTP_PASSWORD"] 
                ?? _configuration["MY_SMTP_APP_PASSWORD"] 
                ?? _configuration["SMTP_APP_PASSWORD"];

            var password = rawPassword?.Replace(" ", "");

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _logger.LogWarning("SMTP settings are missing or incomplete in configuration. Email was not sent.");
                return;
            }

            int port = int.TryParse(portString, out var parsedPort) ? parsedPort : 587;
            bool enableSsl = !bool.TryParse(enableSslString, out var parsedSsl) || parsedSsl;

            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail ?? username, senderName),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            await smtpClient.SendMailAsync(message);
        }

        public async Task SendFlashcardReminderEmailAsync(string toEmail, string username, string? customSubject = null, string? customBody = null)
        {
            string defaultSubject = "Gmitrzak English Academy – Time for your flashcards review! 📇";
            string defaultBodyHtml = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; color: #333;'>
                    <h2>Hello {{username}}! 👋</h2>
                    <p>We noticed that you haven't reviewed your flashcards for at least 3 days.</p>
                    <p>Consistency is key to mastering the English language! Log in to the platform and complete your daily review session.</p>
                    <br/>
                    <a href='https://www.gmitrzak-english-academy.pl' 
                       style='background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>
                        Go to Flashcards 🚀
                    </a>
                    <br/><br/>
                    <p>Best of luck with your studies,<br/>The Gmitrzak English Academy Team</p>
                </div>";

            string rawSubject = !string.IsNullOrWhiteSpace(customSubject) ? customSubject : defaultSubject;
            string rawBody = !string.IsNullOrWhiteSpace(customBody) ? customBody : defaultBodyHtml;

            string subject = rawSubject.Replace("{username}", username);
            string bodyHtml = rawBody.Replace("{username}", username);

            await SendEmailAsync(toEmail, subject, bodyHtml);
        }
    }
}
