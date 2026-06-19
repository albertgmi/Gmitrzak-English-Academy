using inzBackend.Entities.Base;
using inzBackend.Entities.Curriculum;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Assignments
{
    public class UserMatrixModuleDueDateOverride : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int MatrixModuleId { get; set; }
        public MatrixModule MatrixModule { get; set; } = null!;
        public DateOnly NewDeadline { get; set; }
    }
}
