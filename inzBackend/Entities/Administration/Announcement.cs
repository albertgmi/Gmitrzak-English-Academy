using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;
using inzBackend.Enums;

namespace inzBackend.Entities.Administration
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
