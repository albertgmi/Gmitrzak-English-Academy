using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace inzBackend.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HttpClient _httpClient = new HttpClient();

        public EmailService(
            IConfiguration configuration, 
            ILogger<EmailService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private (string AcademyName, string DomainUrl) GetBrandInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string hostOrOrigin = string.Empty;

            if (httpContext != null)
            {
                if (httpContext.Request.Headers.TryGetValue("Origin", out var originValues))
                {
                    hostOrOrigin = originValues.ToString();
                }
                else if (httpContext.Request.Headers.TryGetValue("Referer", out var refererValues))
                {
                    hostOrOrigin = refererValues.ToString();
                }
                else if (httpContext.Request.Host.HasValue)
                {
                    hostOrOrigin = httpContext.Request.Host.Value;
                }
            }

            if (hostOrOrigin.ToLower().Contains("bankowska"))
            {
                return ("Bańkowska English Academy", "https://www.bankowska-english-academy.pl");
            }

            return ("Gmitrzak English Academy", "https://www.gmitrzak-english-academy.pl");
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var brevoApiKey = _configuration["BREVO_API_KEY"] 
                ?? _configuration["BrevoApiKey"]
                ?? _configuration["SmtpSettings:BrevoApiKey"]
                ?? _configuration["SmtpSettings__BrevoApiKey"];

            var senderEmail = _configuration["SmtpSettings:SenderEmail"] 
                ?? _configuration["SMTP_SENDER_EMAIL"] 
                ?? _configuration["SmtpSettings:Username"] 
                ?? _configuration["SMTP_USERNAME"] 
                ?? "piotr.gmitrzak@gmail.com";

            var (academyName, _) = GetBrandInfo();

            var senderName = _configuration["SmtpSettings:SenderName"] 
                ?? _configuration["SMTP_SENDER_NAME"] 
                ?? academyName;

            if (string.IsNullOrEmpty(brevoApiKey))
            {
                _logger.LogWarning("BREVO_API_KEY environment variable is missing on Railway.");
                throw new InvalidOperationException("BREVO_API_KEY is missing in Railway environment variables. Please add BREVO_API_KEY to enable email sending on Railway.");
            }

            _logger.LogInformation($"Sending email to {toEmail} via Brevo HTTP API (Port 443)...");

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("api-key", brevoApiKey);

            var payload = new
            {
                sender = new { name = senderName, email = senderEmail },
                to = new[] { new { email = toEmail } },
                subject = subject,
                htmlContent = bodyHtml
            };

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Brevo HTTP API Error: {response.StatusCode} - {responseText}");
                    throw new InvalidOperationException($"Brevo API Error ({response.StatusCode}): {responseText}");
                }

                _logger.LogInformation($"Successfully dispatched email to {toEmail} via Brevo HTTP API.");
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                _logger.LogError(ex, $"Failed to send email via Brevo HTTP API: {ex.Message}");
                throw new InvalidOperationException($"Brevo HTTP API dispatch failed: {ex.Message}", ex);
            }
        }

        public async Task SendFlashcardReminderEmailAsync(string toEmail, string username, string? customSubject = null, string? customBody = null)
        {
            var (academyName, domainUrl) = GetBrandInfo();

            string defaultSubject = $"{academyName}: Time for your flashcards review!";
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
                            <h1 style='color: #2563eb; margin: 0; font-size: 22px;'>{academyName}</h1>
                        </div>
                        <h2 style='color: #0f172a; font-size: 18px; margin-top: 0;'>Hello {username}! 👋</h2>
                        <div style='font-size: 15px; color: #334155;'>
                            {formattedParagraphs}
                        </div>
                        <div style='text-align: center; margin: 28px 0;'>
                            <a href='{domainUrl}' 
                               style='background-color: #2563eb; color: #ffffff; padding: 14px 28px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block; font-size: 15px;'>
                                Go to Flashcards 🚀
                            </a>
                        </div>
                        <hr style='border: none; border-top: 1px solid #f1f5f9; margin: 24px 0;'/>
                        <p style='font-size: 13px; color: #64748b; margin: 0; text-align: center;'>
                            Best of luck with your studies,<br/><strong>The {academyName} Team</strong>
                        </p>
                    </div>";
            }

            await SendEmailAsync(toEmail, subject, formattedContent);
        }
    }
}
