namespace inzBackend.Models
{
    public class CatalogueEntry : BaseEntity
    {
        public int CatalogueId { get; set; }
        public Catalogue Catalogue { get; set; } = null!;
        public DateOnly EntryDate { get; set; }
        public string UserRef { get; set; } = string.Empty;
        public string Entry { get; set; } = string.Empty;
        public string TranslatedEntry { get; set; } = string.Empty;
        public string ComputedKey { get; set; } = string.Empty;
    }
}
