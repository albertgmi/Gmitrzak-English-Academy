using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;
using inzBackend.Enums;

namespace inzBackend.Entities.Resources
{
    public class WordfinderCatalogue : AuditableEntity
    {
        public int StudentUserId { get; set; }
        public AppUser StudentUser { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public WordfinderCatalogueStatus Status { get; set; } = WordfinderCatalogueStatus.Draft;
        public string? RejectionReason { get; set; }
        public int? ApprovedCatalogueId { get; set; }
        public Catalogue? ApprovedCatalogue { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
        public ICollection<WordfinderCatalogueEntry> Entries { get; set; } = new List<WordfinderCatalogueEntry>();
    }
}
