namespace inzBackend.Models
{
    public class Memory : AuditableEntity
    {
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
