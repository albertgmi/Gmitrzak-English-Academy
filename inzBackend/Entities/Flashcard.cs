namespace inzBackend.Models
{
    public class Flashcard : AuditableEntity
    {
        public int UserId { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int EaseFactor { get; set; }
        public int Interval { get; set; }
        public bool IsLeech { get; set; }
        public DateOnly NextReviewDate { get; set; }
    }
}
