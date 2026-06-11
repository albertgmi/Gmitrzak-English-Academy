namespace inzBackend.Models
{
    public class UserMatrixAssignment : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int MatrixId { get; set; }
        public Matrix Matrix { get; set; } = null!;
        public DateOnly StartDate { get; set; }
    }
}
