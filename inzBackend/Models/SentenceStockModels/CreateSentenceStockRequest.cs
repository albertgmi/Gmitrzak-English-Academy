namespace inzBackend.Models.SentenceStockModels
{
    public class CreateSentenceStockRequest
    {
        public string Polish { get; set; } = string.Empty;
        public string EnglishTranslation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
