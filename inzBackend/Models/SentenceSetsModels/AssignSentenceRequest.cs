namespace inzBackend.Models.SentenceSetsModels
{
    public class AssignSentenceRequest
    {
        public int UserId { get; set; }
        public int? SentenceSetId { get; set; }
        public int? SentenceStockId { get; set; }
        public string DueDate { get; set; } = string.Empty;
    }
}
