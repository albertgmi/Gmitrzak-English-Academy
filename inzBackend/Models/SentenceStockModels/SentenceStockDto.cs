namespace inzBackend.Models.SentenceStockModels
{
    public class SentenceStockDto
    {
        public int Id { get; set; }
        public string Polish { get; set; } = string.Empty;
        public string EnglishTranslation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
