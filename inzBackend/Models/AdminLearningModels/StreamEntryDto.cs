namespace inzBackend.Models.AdminLearningModels
{
    public class StreamEntryDto
    {
        public int Id { get; set; }
        public string Command { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTimeOffset ExecutedAt { get; set; }
    }
}
