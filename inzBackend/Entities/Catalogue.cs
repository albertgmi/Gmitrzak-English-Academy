namespace inzBackend.Models
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
