namespace inzBackend.Models.SentenceSetsModels
{
    public class CreateSentenceSetRequest
    {
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<int> SentenceStockIds { get; set; } = [];
    }
}
