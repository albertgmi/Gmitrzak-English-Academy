namespace inzBackend.Services.EmailServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string bodyHtml);
        Task SendFlashcardReminderEmailAsync(string toEmail, string username, string? customSubject = null, string? customBody = null);
    }
}
