using inzBackend.Entities.Base;
using inzBackend.Entities.Resources;
using inzBackend.Entities.SpacedRepetition;

namespace inzBackend.Entities.LearningMaterials
{
    public class Vocabulary : AuditableEntity
    {
        public string Front { get; set; } = string.Empty; 
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int? CatalogueId { get; set; }
        public Catalogue? Catalogue { get; set; }
        public IEnumerable<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}
