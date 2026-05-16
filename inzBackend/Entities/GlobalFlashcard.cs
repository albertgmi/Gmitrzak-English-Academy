using inzBackend.Models;

namespace inzBackend.Entities
{
    public class GlobalFlashcard : AuditableEntity
    {
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
