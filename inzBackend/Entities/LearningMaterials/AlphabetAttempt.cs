using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.LearningMaterials
{
    public class AlphabetAttempt : BaseEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int AlphabetEntryId { get; set; }
        public AlphabetEntry AlphabetEntry { get; set; } = null!;
        public string ProblemLetters { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
    }
}
