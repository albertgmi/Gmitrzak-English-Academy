namespace inzBackend.Models
{
    public class Sentence : AuditableEntity
    {
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
