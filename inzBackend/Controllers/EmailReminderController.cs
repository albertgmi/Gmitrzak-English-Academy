using inzBackend.Entities.Identity;
using inzBackend.Enums;
using inzBackend.Helpers;
using inzBackend.Models;
using inzBackend.Models.UserModels;
using inzBackend.Services.EmailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Controllers
{
    [Route("api/email-reminder")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class EmailReminderController : ControllerBase
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailReminderController> _logger;

        public EmailReminderController(
            GmitrzakEnglishAcademyDbContext dbContext,
            IEmailService emailService,
            ILogger<EmailReminderController> logger)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet("students")]
        public async Task<ActionResult<List<FlashcardInactiveUserDto>>> GetStudentsForReminder()
        {
            var today = PolandTime.Today;

            var students = await _dbContext.Users
                .Where(u => u.Role == UserRole.User && u.IsActive)
                .ToListAsync();

            var userIds = students.Select(s => s.Id).ToList();

            var latestStudyLogs = await _dbContext.FlashcardStudyLogs
                .Where(log => userIds.Contains(log.UserId))
                .GroupBy(log => log.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    LastStudyDate = g.Max(x => x.StudyDate)
                })
                .ToDictionaryAsync(x => x.UserId, x => x.LastStudyDate);

            var result = students.Select(student =>
            {
                DateOnly? lastStudyDate = latestStudyLogs.TryGetValue(student.Id, out var date) ? date : null;
                int daysInactive = lastStudyDate.HasValue
                    ? today.DayNumber - lastStudyDate.Value.DayNumber
                    : 999;

                return new FlashcardInactiveUserDto
                {
                    Id = student.Id,
                    Username = student.Username,
                    Email = student.Email,
                    LastActiveAt = student.LastActiveAt,
                    LastFlashcardStudyDate = lastStudyDate,
                    DaysInactive = daysInactive,
                    IsInactiveForThreeDays = daysInactive >= 3
                };
            })
            .OrderByDescending(x => x.IsInactiveForThreeDays)
            .ThenByDescending(x => x.DaysInactive)
            .ToList();

            return Ok(result);
        }

        [HttpPost("send-flashcard-reminders")]
        public async Task<ActionResult<SendRemindersResultDto>> SendFlashcardReminders([FromBody] SendFlashcardRemindersRequest request)
        {
            var result = new SendRemindersResultDto();

            List<AppUser> targetUsers;

            if (request.UserIds != null && request.UserIds.Count > 0)
            {
                targetUsers = await _dbContext.Users
                    .Where(u => request.UserIds.Contains(u.Id) && u.IsActive)
                    .ToListAsync();
            }
            else
            {
                var today = PolandTime.Today;
                var threeDaysAgoCutoff = today.AddDays(-3);

                var activeUserIds = await _dbContext.FlashcardStudyLogs
                    .Where(log => log.StudyDate > threeDaysAgoCutoff)
                    .Select(log => log.UserId)
                    .Distinct()
                    .ToListAsync();

                targetUsers = await _dbContext.Users
                    .Where(u => u.Role == UserRole.User && u.IsActive && !activeUserIds.Contains(u.Id))
                    .ToListAsync();
            }

            foreach (var user in targetUsers)
            {
                try
                {
                    await _emailService.SendFlashcardReminderEmailAsync(user.Email, user.Username, request.CustomSubject, request.CustomBody);
                    result.SentCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send reminder email to user {user.Username} ({user.Email})");
                    result.FailedCount++;
                    result.Errors.Add($"Failed for {user.Username}: {ex.Message}");
                }
            }

            return Ok(result);
        }
    }
}
