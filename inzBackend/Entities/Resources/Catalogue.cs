using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Resources
{
    public class Catalogue : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateOnly UploadedDate { get; set; }
        public int UploadedByUserId { get; set; }
        public AppUser UploadedBy { get; set; } = null!;
        public IEnumerable<CatalogueEntry> Entries { get; set; } = new List<CatalogueEntry>();
    }
}
