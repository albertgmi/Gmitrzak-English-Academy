namespace inzBackend.Models.StreamEntryModels
{
    public class StreamEntryListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Command { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTimeOffset ExecutedAt { get; set; }
    }
}
