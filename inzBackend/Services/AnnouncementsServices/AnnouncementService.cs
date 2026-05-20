using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.AnnouncementModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;

namespace inzBackend.Services.AnnouncementsServices
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public AnnouncementService(
            GmitrzakEnglishAcademyDbContext dbContext,
            IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
        }

        public List<AnnouncementDto> getAll()
        {
            return _dbContext.Announcements
                .Include(x => x.Sender)
                .Include(x => x.Recipients)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new AnnouncementDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Content = x.Content,
                    SenderUsername = x.Sender.Username,
                    CreatedAt = x.CreatedAt,
                    TotalRecipients = x.Recipients.Count(),
                    ReadCount = x.Recipients.Count(r => r.IsRead)
                })
                .ToList();
        }

        public List<AnnouncementInboxDto> getInbox()
        {
            var userId = _userContextService.GetUserId;
            return _dbContext.AnnouncementRecipients
                .Include(x => x.Announcement).ThenInclude(a => a.Sender)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Announcement.CreatedAt)
                .Select(x => new AnnouncementInboxDto
                {
                    Id = x.Id,
                    AnnouncementId = x.AnnouncementId,
                    Title = x.Announcement.Title,
                    Content = x.Announcement.Content,
                    SenderUsername = x.Announcement.Sender.Username,
                    CreatedAt = x.Announcement.CreatedAt,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt
                })
                .ToList();
        }

        public UnreadCountDto getUnreadCount()
        {
            var userId = _userContextService.GetUserId;
            var count = _dbContext.AnnouncementRecipients
                .Count(x => x.UserId == userId && !x.IsRead);
            return new UnreadCountDto { Count = count };
        }

        public void create(CreateAnnouncementRequest request)
        {
            var senderId = _userContextService.GetUserId!.Value;

            var announcement = new Announcement
            {
                SenderId = senderId,
                Title = request.Title,
                Content = request.Content
            };

            _dbContext.Announcements.Add(announcement);
            _dbContext.SaveChanges();

            List<int> recipientIds;
            if (request.RecipientUserIds is null || !request.RecipientUserIds.Any())
            {
                recipientIds = _dbContext.Users
                    .Where(x => x.Role == Enums.UserRole.User && x.IsActive)
                    .Select(x => x.Id)
                    .ToList();
            }
            else
            {
                recipientIds = request.RecipientUserIds;
            }

            var recipients = recipientIds.Select(uid => new AnnouncementRecipient
            {
                AnnouncementId = announcement.Id,
                UserId = uid,
                IsRead = false
            }).ToList();

            _dbContext.AnnouncementRecipients.AddRange(recipients);
            _dbContext.SaveChanges();
        }

        public void markRead(int recipientId)
        {
            var userId = _userContextService.GetUserId;
            var recipient = _dbContext.AnnouncementRecipients
                .FirstOrDefault(x => x.Id == recipientId && x.UserId == userId);
            if (recipient is null) return;
            recipient.IsRead = true;
            recipient.ReadAt = DateTimeOffset.UtcNow;
            _dbContext.SaveChanges();
        }

        public void markAllRead()
        {
            var userId = _userContextService.GetUserId;
            var unread = _dbContext.AnnouncementRecipients
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToList();
            foreach (var r in unread)
            {
                r.IsRead = true;
                r.ReadAt = DateTimeOffset.UtcNow;
            }
            _dbContext.SaveChanges();
        }

        public void delete(int id)
        {
            var announcement = _dbContext.Announcements
                .Include(x => x.Recipients)
                .FirstOrDefault(x => x.Id == id);
            if (announcement is null)
                throw new NotFoundException("Announcement not found");

            _dbContext.AnnouncementRecipients.RemoveRange(announcement.Recipients);
            _dbContext.Announcements.Remove(announcement);
            _dbContext.SaveChanges();
        }
    }
}
