namespace inzBackend.Models.StreamEntryModels
{
    public class CreateStreamEntryRequest
    {
        public int UserId { get; set; }
        public string Command { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
