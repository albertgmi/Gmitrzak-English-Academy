using inzBackend.Enums;

namespace inzBackend.Models
{
    public class PronunciationEntry : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public string Word { get; set; } = string.Empty;
        public PronunciationStatus Status { get; set; } = PronunciationStatus.Pending;
        public int SortOrder { get; set; }
        public DateOnly? MarkedCorrectAt { get; set; }
        public bool IsInCurrentSession { get; set; } = false;
    }
}
