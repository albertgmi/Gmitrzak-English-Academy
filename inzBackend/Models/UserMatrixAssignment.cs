namespace inzBackend.Models
{
    public class UserMatrixAssignment : AuditableEntity
    {
        public int UserId { get; set; }
        public int MatrixId { get; set; }
        public DateOnly StartDate { get; set; }
    }
}
