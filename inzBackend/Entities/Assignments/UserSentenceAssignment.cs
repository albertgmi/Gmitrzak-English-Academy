using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;
using inzBackend.Entities.LearningMaterials;

namespace inzBackend.Entities.Assignments
{
    public class UserSentenceAssignment : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int? SentenceSetId { get; set; }
        public SentenceSet? SentenceSet { get; set; }
        public int? SentenceStockId { get; set; }
        public SentenceStock? SentenceStock { get; set; }
        public DateOnly DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public IEnumerable<UserSentenceAnswer> Answers { get; set; } = [];
        public bool IsReviewed { get; set; } = false;
    }
}
