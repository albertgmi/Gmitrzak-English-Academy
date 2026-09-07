using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;
using inzBackend.Enums;
using inzBackend.Helpers;

namespace inzBackend.Entities.LearningMaterials
{
    public class IrregularVerb : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public string PolishTranslation { get; set; } = string.Empty;
        public string EnglishForms { get; set; } = string.Empty;
        public IrregularVerbLevel Level { get; set; }
        public int EaseFactor { get; set; } = 250;
        public int Interval { get; set; } = 0;
        public bool IsLeech { get; set; } = false;
        public DateOnly NextReviewDate { get; set; } = PolandTime.Today;
    }
}
