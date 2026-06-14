using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Administration
{
    public class AnnouncementRecipient : BaseEntity
    {
        public int AnnouncementId { get; set; }
        public Announcement Announcement { get; set; } = null!;
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public bool IsRead { get; set; }
        public DateTimeOffset? ReadAt { get; set; }
        public bool? SignedUp { get; set; }
        public bool? Vote { get; set; }
    }
}
