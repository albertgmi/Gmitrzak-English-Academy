namespace inzBackend.Models.CatalogueModels
{
    public class CatalogueDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly UploadedDate { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public int EntryCount { get; set; }
    }
}
