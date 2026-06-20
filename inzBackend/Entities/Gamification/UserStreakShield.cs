using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Gamification
{
    public class UserStreakShield : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public bool IsUsed { get; set; } = false;
        public DateOnly? ProtectedDate { get; set; }
    }
}
