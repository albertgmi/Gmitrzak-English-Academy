using inzBackend.Entities.Base;
using inzBackend.Entities.Curriculum;

namespace inzBackend.Entities.LearningMaterials
{
    public class ModuleSentenceSet : BaseEntity
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public int SentenceSetId { get; set; }
        public SentenceSet SentenceSet { get; set; } = null!;
    }
}
