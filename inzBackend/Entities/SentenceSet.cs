using inzBackend.Models;

namespace inzBackend.Entities
{
    public class SentenceSet : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int Order { get; set; }
        public ICollection<SentenceSetItem> Items { get; set; } = [];
        public ICollection<UserSentenceAssignment> Assignments { get; set; } = [];
    }
}
