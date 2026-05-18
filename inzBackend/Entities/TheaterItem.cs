using inzBackend.Enums;
using inzBackend.Models;

namespace inzBackend.Entities
{
    public class TheaterItem : AuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public int DurationMinutes { get; set; }
        public string Level { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
