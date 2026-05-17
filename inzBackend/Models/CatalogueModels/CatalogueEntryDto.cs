namespace inzBackend.Models.CatalogueModels
{
    public class CatalogueEntryDto
    {
        public int Id { get; set; }
        public DateOnly EntryDate { get; set; }
        public string UserRef { get; set; } = string.Empty;
        public string Entry { get; set; } = string.Empty;
        public string ComputedKey { get; set; } = string.Empty;
        public string CatalogueName { get; set; } = string.Empty;
    }
}
