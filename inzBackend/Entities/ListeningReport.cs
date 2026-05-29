using inzBackend.Enums;

namespace inzBackend.Models
{
    public class ListeningReport : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public DateOnly ReportDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public int EpisodeCount { get; set; }
    }
}
