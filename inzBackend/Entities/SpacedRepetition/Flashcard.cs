using inzBackend.Entities.Base;
using inzBackend.Entities.LearningMaterials;

namespace inzBackend.Entities.SpacedRepetition
{
    public class Flashcard : AuditableEntity
    {
        public int UserId { get; set; }
        public int VocabularyId { get; set; }
        public Vocabulary Vocabulary { get; set; } = null!;
        public int EaseFactor { get; set; }
        public int Interval { get; set; }
        public bool IsLeech { get; set; }
        public DateOnly NextReviewDate { get; set; }
    }
}
