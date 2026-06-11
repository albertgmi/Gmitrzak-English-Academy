using inzBackend.Models;

namespace inzBackend.Entities
{
    public class UserLoginLog : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public DateOnly LoginDate { get; set; }
        public DateTimeOffset LoginAt { get; set; }
    }
}
