using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net;

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
                _logger.LogWarning("Gmail SMTP settings missing in configuration. Email not sent.");
                throw new InvalidOperationException("Gmail SMTP credentials (SmtpSettings__Username / SmtpSettings__Password) are missing in Railway Environment Variables.");
            }

            int primaryPort = int.TryParse(portString, out var parsedPort) ? parsedPort : 587;
            bool enableSsl = !bool.TryParse(enableSslString, out var parsedSsl) || parsedSsl;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail ?? username));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = bodyHtml };
            message.Body = bodyBuilder.ToMessageBody();

            int[] portsToTry = primaryPort == 587 ? new[] { 587, 465 } : new[] { primaryPort, 587 };
            Exception? lastException = null;

            // Resolve IPv4 addresses to bypass Linux IPv6 DNS blackholes on Railway
            IPAddress? ipv4Address = null;
            try
            {
                var dnsAddresses = await Dns.GetHostAddressesAsync(host);
                ipv4Address = dnsAddresses.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"DNS lookup for {host} failed: {ex.Message}");
            }

            foreach (var currentPort in portsToTry)
            {
                using var client = new SmtpClient();
                // 20 second timeout for Gmail TLS handshake
                client.Timeout = 20000;
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                try
                {
                    var secureOption = enableSsl
                        ? (currentPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls)
                        : SecureSocketOptions.None;

                    _logger.LogInformation($"Attempting Gmail SMTP connection to {host}:{currentPort} ({ipv4Address?.ToString() ?? "default"}) with option {secureOption}...");

                    if (ipv4Address != null)
                    {
                        await client.ConnectAsync(ipv4Address.ToString(), currentPort, secureOption);
                    }
                    else
                    {
                        await client.ConnectAsync(host, currentPort, secureOption);
                    }

                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);

                    _logger.LogInformation($"Successfully sent Gmail email to {toEmail} via port {currentPort}");
                    return;
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    _logger.LogWarning($"Gmail SMTP attempt failed on port {currentPort}: {ex.Message}");
                }
            }

            _logger.LogError(lastException, $"All Gmail SMTP attempts failed for {toEmail}. Check Railway variables and Google App Password.");
            throw new InvalidOperationException($"Gmail SMTP Connection Failed: {lastException?.Message}. Check SmtpSettings__Username ({username}) and Google App Password on Railway.", lastException);
        }

        public async Task SendFlashcardReminderEmailAsync(string toEmail, string username, string? customSubject = null, string? customBody = null)
        {
            string defaultSubject = "Gmitrzak English Academy – Time for your flashcards review! 📇";
            string defaultPlainText = "We noticed that you haven't reviewed your flashcards for at least 3 days.\n\nConsistency is key to mastering the English language! Log in to the platform and complete your daily review session.";

            string rawSubject = !string.IsNullOrWhiteSpace(customSubject) ? customSubject : defaultSubject;
            string rawMessage = !string.IsNullOrWhiteSpace(customBody) ? customBody : defaultPlainText;

            string subject = rawSubject.Replace("{username}", username);

            string formattedContent;
            if (rawMessage.Contains("<div") || rawMessage.Contains("<p>") || rawMessage.Contains("<br"))
            {
                formattedContent = rawMessage.Replace("{username}", username);
            }
            else
            {
                string textWithUsername = rawMessage.Replace("{username}", username);
                var paragraphs = textWithUsername
                    .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                    .Select(p => WebUtility.HtmlEncode(p))
                    .Select(p => string.IsNullOrWhiteSpace(p) ? "<br/>" : $"<p style='margin: 0 0 12px 0; line-height: 1.6;'>{p}</p>");

                string formattedParagraphs = string.Join("", paragraphs);

                formattedContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 24px; border: 1px solid #e2e8f0; border-radius: 12px; background-color: #ffffff; color: #1e293b;'>
                        <div style='text-align: center; padding-bottom: 16px; border-bottom: 2px solid #f1f5f9; margin-bottom: 20px;'>
                            <h1 style='color: #2563eb; margin: 0; font-size: 22px;'>Gmitrzak English Academy</h1>
                        </div>
                        <h2 style='color: #0f172a; font-size: 18px; margin-top: 0;'>Hello {username}! 👋</h2>
                        <div style='font-size: 15px; color: #334155;'>
                            {formattedParagraphs}
                        </div>
                        <div style='text-align: center; margin: 28px 0;'>
                            <a href='https://www.gmitrzak-english-academy.pl' 
                               style='background-color: #2563eb; color: #ffffff; padding: 14px 28px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; font-size: 15px;'>
                                Go to Flashcards 🚀
                            </a>
                        </div>
                        <hr style='border: none; border-top: 1px solid #f1f5f9; margin: 24px 0;'/>
                        <p style='font-size: 13px; color: #64748b; margin: 0; text-align: center;'>
                            Best of luck with your studies,<br/><strong>The Gmitrzak English Academy Team</strong>
                        </p>
                    </div>";
            }

            await SendEmailAsync(toEmail, subject, formattedContent);
        }
    }
}
