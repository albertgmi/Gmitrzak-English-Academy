namespace inzBackend.Models.CreditModels
{
    public class CreditHistoryItemDto
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
