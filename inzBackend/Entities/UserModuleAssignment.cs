using inzBackend.Models;

namespace inzBackend.Entities
{
    public class UserModuleAssignment : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public DateOnly DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
