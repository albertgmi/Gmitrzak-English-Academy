using inzBackend.Entities;
using inzBackend.Exceptions;
using inzBackend.Models.AnnouncementModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using inzBackend.Helpers;
using inzBackend.Enums;
using inzBackend.Entities.Administration;

namespace inzBackend.Services.AnnouncementsServices
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly GmitrzakEnglishAcademyDbContext _dbContext;
        private readonly IUserContextService _userContextService;

        public AnnouncementService(GmitrzakEnglishAcademyDbContext dbContext, IUserContextService userContextService)
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
                    ReadCount = x.Recipients.Count(r => r.IsRead),
                    Type = x.Type.ToString(),
                    SignUpCount = x.Recipients.Count(r => r.SignedUp == true),
                    VoteYesCount = x.Recipients.Count(r => r.Vote == true),
                    VoteNoCount = x.Recipients.Count(r => r.Vote == false)
                })
                .ToList();
        }

        public List<AnnouncementInboxDto> getInbox()
        {
            var userId = _userContextService.GetUserId;
            return _dbContext.AnnouncementRecipients
                .Include(x => x.Announcement)
                    .ThenInclude(a => a.Sender)
                        .ThenInclude(s => s.Profile)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Announcement.CreatedAt)
                .Select(x => new AnnouncementInboxDto
                {
                    Id = x.Id,
                    AnnouncementId = x.AnnouncementId,
                    Title = x.Announcement.Title,
                    Content = x.Announcement.Content,
                    SenderUsername = x.Announcement.Sender.Username,
                    SenderAvatarUrl = x.Announcement.Sender.Profile.AvatarUrl,
                    CreatedAt = x.Announcement.CreatedAt,
                    IsRead = x.IsRead,
                    ReadAt = x.ReadAt,
                    Type = x.Announcement.Type.ToString(),
                    SignedUp = x.SignedUp,
                    Vote = x.Vote
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

            if (!Enum.TryParse<AnnouncementType>(request.Type, true, out var announcementType))
            {
                announcementType = AnnouncementType.Announcement;
            }

            var announcement = new Announcement
            {
                SenderId = senderId,
                Title = request.Title,
                Content = request.Content,
                Type = announcementType
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

        public void signUp(int recipientId)
        {
            var userId = _userContextService.GetUserId;
            var recipient = _dbContext.AnnouncementRecipients
                .Include(x => x.Announcement)
                .FirstOrDefault(x => x.Id == recipientId && x.UserId == userId);

            if (recipient is null || recipient.Announcement.Type != AnnouncementType.Listing) return;

            recipient.SignedUp = !(recipient.SignedUp ?? false);
            _dbContext.SaveChanges();
        }

        public void vote(int recipientId, bool voteValue)
        {
            var userId = _userContextService.GetUserId;
            var recipient = _dbContext.AnnouncementRecipients
                .Include(x => x.Announcement)
                .FirstOrDefault(x => x.Id == recipientId && x.UserId == userId);

            if (recipient is null || recipient.Announcement.Type != AnnouncementType.Voting) return;

            recipient.Vote = voteValue;
            _dbContext.SaveChanges();
        }

        public void markRead(int recipientId)
        {
            var userId = _userContextService.GetUserId;
            var recipient = _dbContext.AnnouncementRecipients
                .FirstOrDefault(x => x.Id == recipientId && x.UserId == userId);
            if (recipient is null) return;
            recipient.IsRead = true;
            recipient.ReadAt = PolandTime.Now;
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
                r.ReadAt = PolandTime.Now;
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

        public AnnouncementDetailsDto getDetails(int announcementId)
        {
            var announcement = _dbContext.Announcements
                .Include(a => a.Recipients)
                    .ThenInclude(r => r.User)
                        .ThenInclude(u => u.Profile)
                .FirstOrDefault(a => a.Id == announcementId);

            if (announcement is null)
                throw new NotFoundException("Announcement not found");

            return new AnnouncementDetailsDto
            {
                AnnouncementId = announcement.Id,
                Title = announcement.Title,
                Type = announcement.Type.ToString(),

                Recipients = announcement.Recipients
                    .Select(r => new AnnouncementRecipientDetailsDto
                    {
                        UserId = r.UserId,
                        Username = r.User.Username,
                        Email = r.User.Email,
                        IsRead = r.IsRead,
                        ReadAt = r.ReadAt,
                        SignedUp = r.SignedUp,
                        Vote = r.Vote,
                        AvatarUrl = r.User.Profile?.AvatarUrl
                    })
                    .OrderBy(x => x.Username)
                    .ToList()
            };
        }
    }
}