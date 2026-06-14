using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Gamification
{
    public class Grade : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public DateOnly GradeDate { get; set; }
        public decimal Percentage { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
