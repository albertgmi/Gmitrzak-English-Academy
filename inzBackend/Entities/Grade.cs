namespace inzBackend.Models
{
    public class Grade : AuditableEntity
    {
        public int UserId { get; set; }
        public DateOnly GradeDate { get; set; }
        public decimal Percentage { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
