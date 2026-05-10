namespace inzBackend.Models
{
    public class StreamEntry : BaseEntity
    {
        public int UserId { get; set; }
        public string Command { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTimeOffset ExecutedAt { get; set; }
    }
}
