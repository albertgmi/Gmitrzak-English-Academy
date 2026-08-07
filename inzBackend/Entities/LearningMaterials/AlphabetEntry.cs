using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;
using inzBackend.Enums;

namespace inzBackend.Entities.LearningMaterials
{
    public class AlphabetEntry : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public DateOnly WeekStartDate { get; set; }
        public AlphabetEntryType Type { get; set; }
        public string Content { get; set; } = string.Empty;
        public int? AbbreviationId { get; set; }
        public AlphabetAbbreviation? Abbreviation { get; set; }
        public PronunciationStatus Status { get; set; } = PronunciationStatus.Pending;
        public int SortOrder { get; set; }
        public DateOnly? MarkedCorrectAt { get; set; }
        public ICollection<AlphabetAttempt> Attempts { get; set; } = new List<AlphabetAttempt>();
    }
}
