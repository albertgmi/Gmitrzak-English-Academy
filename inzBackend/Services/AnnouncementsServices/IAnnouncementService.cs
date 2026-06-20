using inzBackend.Models.AnnouncementModels;

namespace inzBackend.Services.AnnouncementsServices
{
    public interface IAnnouncementService
    {
        List<AnnouncementDto> GetAll();
        List<AnnouncementInboxDto> GetInbox();
        UnreadCountDto GetUnreadCount();
        void Create(CreateAnnouncementRequest request);
        void MarkRead(int recipientId);
        void MarkAllRead();
        void Delete(int id);
        void Vote(int recipientId, bool voteValue);
        void SignUp(int recipientId);
        AnnouncementDetailsDto GetDetails(int announcementId);
    }
}
