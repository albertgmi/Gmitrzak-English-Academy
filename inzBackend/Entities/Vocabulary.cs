using inzBackend.Models;

namespace inzBackend.Entities
{
    public class Vocabulary : AuditableEntity
    {
        public string Front { get; set; } = string.Empty; 
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public IEnumerable<Flashcard> Flashcards { get; set; } = new List<Flashcard>();
    }
}
