using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;
using inzBackend.Enums;
using inzBackend.Helpers;

namespace inzBackend.Entities.Curriculum
{
    public class AcademyExamSignup : AuditableEntity
    {
        public int ExamId { get; set; }
        public AcademyExam Exam { get; set; } = null!;

        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public DateTime SignedUpAt { get; set; } = PolandTime.DateTimeNow;
        public ExamSignupStatus Status { get; set; } = ExamSignupStatus.Registered;
    }
}
