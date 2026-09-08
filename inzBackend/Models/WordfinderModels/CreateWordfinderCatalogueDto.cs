namespace inzBackend.Models.WordfinderModels
{
    public class CreateWordfinderCatalogueDto
    {
        public string Name { get; set; } = string.Empty;
        public List<WordfinderCatalogueEntryDto>? Entries { get; set; }
    }
}
