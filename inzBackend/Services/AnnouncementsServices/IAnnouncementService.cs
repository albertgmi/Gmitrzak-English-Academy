using inzBackend.Models.AnnouncementModels;

namespace inzBackend.Services.AnnouncementsServices
{
    public interface IAnnouncementService
    {
        List<AnnouncementDto> getAll();
        List<AnnouncementInboxDto> getInbox();
        UnreadCountDto getUnreadCount();
        void create(CreateAnnouncementRequest request);
        void markRead(int recipientId);
        void markAllRead();
        void delete(int id);
        void vote(int recipientId, bool voteValue);
        void signUp(int recipientId);
        AnnouncementDetailsDto getDetails(int announcementId);
    }
}
