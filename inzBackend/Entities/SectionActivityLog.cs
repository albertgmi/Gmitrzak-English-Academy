using inzBackend.Models;

namespace inzBackend.Entities
{
    public class SectionActivityLog : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public string Section { get; set; } = string.Empty;
        public DateOnly ActivityDate { get; set; }
    }
}
