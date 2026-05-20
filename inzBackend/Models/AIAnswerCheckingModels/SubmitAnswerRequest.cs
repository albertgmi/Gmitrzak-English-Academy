namespace inzBackend.Models.AIAnswerCheckingModels
{
    public class SubmitAnswerRequest
    {
        public int AssignmentId { get; set; }
        public int SentenceStockId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
    }
}
