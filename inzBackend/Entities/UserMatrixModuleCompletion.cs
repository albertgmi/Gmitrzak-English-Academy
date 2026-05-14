using inzBackend.Models;

namespace inzBackend.Entities
{
    public class UserMatrixModuleCompletion : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int MatrixModuleId { get; set; }
        public MatrixModule MatrixModule { get; set; } = null!;
        public DateOnly CompletedDate { get; set; }
    }
}
