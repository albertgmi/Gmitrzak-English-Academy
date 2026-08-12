using inzBackend.Entities;
using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.EmailServices
{
    public class FlashcardReminderBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<FlashcardReminderBackgroundService> _logger;

        private readonly DayOfWeek[] _targetDays = [DayOfWeek.Tuesday, DayOfWeek.Friday];

        public FlashcardReminderBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<FlashcardReminderBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FlashcardReminderBackgroundService started (scheduled twice a week at 08:00 AM).");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRun();
                _logger.LogInformation($"Next flashcard email reminder execution scheduled in {delay.TotalHours:F2} hours ({delay.TotalMinutes:F0} minutes).");

                await Task.Delay(delay, stoppingToken);

                try
                {
                    await CheckAndSendFlashcardRemindersAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during execution of flashcard email reminder background task.");
                }
            }
        }

        private TimeSpan GetDelayUntilNextRun()
        {
            var now = PolandTime.DateTimeNow;

            for (int dayOffset = 0; dayOffset <= 7; dayOffset++)
            {
                var candidate = now.Date.AddDays(dayOffset).AddHours(8);

                if (_targetDays.Contains(candidate.DayOfWeek) && candidate > now)
                {
                    return candidate - now;
                }
            }

            return TimeSpan.FromHours(1);
        }

        private async Task CheckAndSendFlashcardRemindersAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GmitrzakEnglishAcademyDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var today = PolandTime.Today;
            var threeDaysAgoCutoff = today.AddDays(-3);

            var activeInLast3DaysUserIds = await dbContext.FlashcardStudyLogs
                .Where(log => log.StudyDate > threeDaysAgoCutoff)
                .Select(log => log.UserId)
                .Distinct()
                .ToListAsync(stoppingToken);

            var studentsToRemind = await dbContext.Users
                .Where(u => u.Role == UserRole.User
                         && u.IsActive
                         && !activeInLast3DaysUserIds.Contains(u.Id))
                .ToListAsync(stoppingToken);

            _logger.LogInformation($"[Flashcard Reminder Job] Found {studentsToRemind.Count} students inactive for at least 3 days.");

            foreach (var student in studentsToRemind)
            {
                try
                {
                    await emailService.SendFlashcardReminderEmailAsync(student.Email, student.Username);
                    _logger.LogInformation($"[Flashcard Reminder Job] Reminder email sent successfully to: {student.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Flashcard Reminder Job] Failed to send email to: {student.Email}");
                }
            }
        }
    }
}
