using inzBackend.Helpers;

namespace inzBackend.Models
{
    public class Sentence : AuditableEntity
    {
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsReviewed { get; set; } = false;
        public int EaseFactor { get; set; } = 250;
        public int Interval { get; set; } = 0;
        public bool IsLeech { get; set; } = false;
        public DateOnly? NextReviewDate { get; set; } = PolandTime.Today;
    }
}
