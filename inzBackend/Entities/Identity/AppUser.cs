using inzBackend.Entities.Administration;
using inzBackend.Entities.Assignments;
using inzBackend.Entities.Base;
using inzBackend.Entities.SpacedRepetition;
using inzBackend.Enums;

namespace inzBackend.Entities.Identity
{
    public class AppUser : AuditableEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public IEnumerable<FlashcardStudyLog> FlashcardStudyLogs { get; set; } = new List<FlashcardStudyLog>();
        public Profile Profile { get; set; }
        public IEnumerable<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<UserModuleAssignment> UserModuleAssignments { get; set; } = new List<UserModuleAssignment>();
        public ICollection<UserMatrixModuleCompletion> UserMatrixModuleCompletions { get; set; } = new List<UserMatrixModuleCompletion>();
    }
}
