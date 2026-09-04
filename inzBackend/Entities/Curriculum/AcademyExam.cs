using inzBackend.Entities.Base;
using inzBackend.Enums;

namespace inzBackend.Entities.Curriculum
{
    public class AcademyExam : AuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamLevel Level { get; set; }
        public string? MaterialsUrl { get; set; }
        public string? MaterialsJson { get; set; }
        public int RewardCredits { get; set; }
        public string PassingThreshold { get; set; } = string.Empty;
        public DateTime SignupDeadline { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<AcademyExamSignup> Signups { get; set; } = new List<AcademyExamSignup>();
    }
}
