using inzBackend.Entities.Base;

namespace inzBackend.Entities.LearningMaterials
{
    public class SentenceStock : AuditableEntity
    {
        public string Polish { get; set; } = string.Empty;
        public string EnglishTranslation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public ICollection<SentenceSetItem> SetItems { get; set; } = [];
    }
}
