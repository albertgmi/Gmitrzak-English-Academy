namespace inzBackend.Models.SentenceSetsModels
{
    public class SentenceSetItemDto
    {
        public int Id { get; set; }
        public int SentenceStockId { get; set; }
        public string Polish { get; set; } = string.Empty;
        public string EnglishTranslation { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
