namespace inzBackend.Models.WordfinderModels
{
    public class UpdateWordfinderCatalogueDto
    {
        public string Name { get; set; } = string.Empty;
        public List<WordfinderCatalogueEntryDto> Entries { get; set; } = new();
    }
}
