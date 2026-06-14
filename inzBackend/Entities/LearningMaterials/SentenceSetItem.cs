using inzBackend.Entities.Base;

namespace inzBackend.Entities.LearningMaterials
{
    public class SentenceSetItem : BaseEntity
    {
        public int SentenceSetId { get; set; }
        public SentenceSet SentenceSet { get; set; } = null!;
        public int SentenceStockId { get; set; }
        public SentenceStock SentenceStock { get; set; } = null!;
        public int Order { get; set; }
    }
}
