using inzBackend.Enums;

namespace inzBackend.Models
{
    public class AppUser : AuditableEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public IEnumerable<FlashcardStudyLog> FlashcardStudyLogs { get; set; } = new List<FlashcardStudyLog>();
    }
}
