using inzBackend.Models;

namespace inzBackend.Entities
{
    public class SentenceSet : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;        // e.g. "Sentences 1.1"
        public string GroupName { get; set; } = string.Empty;   // e.g. "Advanced 1"
        public int Order { get; set; }
        public ICollection<SentenceSetItem> Items { get; set; } = [];
        public ICollection<UserSentenceAssignment> Assignments { get; set; } = [];
    }
}
