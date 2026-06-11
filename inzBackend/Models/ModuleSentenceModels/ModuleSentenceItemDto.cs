namespace inzBackend.Models.ModuleSentenceModels
{
    public class ModuleSentenceItemDto
    {
        public int SentenceStockId { get; set; }
        public string Polish { get; set; } = string.Empty;
        public int Order { get; set; }
        public string? PreviousAnswer { get; set; }
        public string? PreviousResult { get; set; }
        public string? PreviousExplanation { get; set; }
        public int? PreviousAnswerId { get; set; }
    }
}
