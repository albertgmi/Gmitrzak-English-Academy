using inzBackend.Models;

namespace inzBackend.Entities
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
