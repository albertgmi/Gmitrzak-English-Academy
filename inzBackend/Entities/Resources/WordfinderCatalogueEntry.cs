using inzBackend.Entities.Base;

namespace inzBackend.Entities.Resources
{
    public class WordfinderCatalogueEntry : BaseEntity
    {
        public int WordfinderCatalogueId { get; set; }
        public WordfinderCatalogue WordfinderCatalogue { get; set; } = null!;
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
    }
}
