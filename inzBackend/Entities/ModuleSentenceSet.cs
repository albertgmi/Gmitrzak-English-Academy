using inzBackend.Models;

namespace inzBackend.Entities
{
    public class ModuleSentenceSet : BaseEntity
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public int SentenceSetId { get; set; }
        public SentenceSet SentenceSet { get; set; } = null!;
    }
}
