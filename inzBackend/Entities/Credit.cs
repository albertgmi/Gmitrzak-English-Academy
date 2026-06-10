using inzBackend.Models;

namespace inzBackend.Entities
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
