using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Gamification
{
    public class Credit : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
    }
}
