namespace inzBackend.Models.CatalogueModels
{
    public class CatalogueEntryFilterRequest
    {
        public string? CatalogueName { get; set; }
        public string? UserRef { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}
