using inzBackend.Models;

namespace inzBackend.Entities
{
    public class TeacherNote : AuditableEntity
    {
        public int TeacherUserId { get; set; }
        public AppUser Teacher { get; set; } = null!;
        public int StudentUserId { get; set; }
        public AppUser Student { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateOnly NoteDate { get; set; }
    }
}
