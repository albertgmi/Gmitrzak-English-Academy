using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.LearningMaterials
{
    public class PronunciationAttempt : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int PronunciationEntryId { get; set; }
        public PronunciationEntry PronunciationEntry { get; set; } = null!;
        public string Feedback { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }
}
