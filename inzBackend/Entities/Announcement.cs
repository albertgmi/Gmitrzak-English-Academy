using inzBackend.Enums;
using inzBackend.Models;

namespace inzBackend.Entities
{
    public class Announcement : AuditableEntity
    {
        public int SenderId { get; set; }
        public AppUser Sender { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public IEnumerable<AnnouncementRecipient> Recipients { get; set; } = new List<AnnouncementRecipient>();
        public AnnouncementType Type { get; set; } = AnnouncementType.Announcement;
    }
}
