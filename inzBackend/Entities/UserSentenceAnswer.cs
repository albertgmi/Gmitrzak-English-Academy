using inzBackend.Models;

namespace inzBackend.Entities
{
    public class UserSentenceAnswer : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int AssignmentId { get; set; }
        public UserSentenceAssignment Assignment { get; set; } = null!;
        public int SentenceStockId { get; set; }
        public SentenceStock SentenceStock { get; set; } = null!;
        public string UserAnswer { get; set; } = string.Empty;
        public string AiResult { get; set; } = string.Empty;
        public string AiExplanation { get; set; } = string.Empty;
        public string? TeacherOverride { get; set; }
        public bool TeacherReviewed { get; set; }
    }
}
