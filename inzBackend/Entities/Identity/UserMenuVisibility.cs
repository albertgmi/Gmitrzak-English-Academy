using inzBackend.Entities.Base;

namespace inzBackend.Entities.Identity
{
    public class UserMenuVisibility : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public string MenuItemKey { get; set; } = string.Empty;
        public bool IsVisible { get; set; } = true;
    }
}
