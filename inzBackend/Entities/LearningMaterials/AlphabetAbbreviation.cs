using inzBackend.Entities.Base;

namespace inzBackend.Entities.LearningMaterials
{
    public class AlphabetAbbreviation : AuditableEntity
    {
        public string Text { get; set; } = string.Empty;
        public ICollection<AlphabetEntry> Entries { get; set; } = new List<AlphabetEntry>();
    }
}
