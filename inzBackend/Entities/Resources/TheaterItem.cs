using inzBackend.Entities.Base;
using inzBackend.Enums;

namespace inzBackend.Entities.Resources
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
