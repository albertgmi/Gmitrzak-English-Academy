namespace inzBackend.Models.AIAnswerCheckingModels
{
    public class SubmitAnswerRequest
    {
        public int ModuleId { get; set; }
        public int SentenceStockId { get; set; }
        public string UserAnswer { get; set; } = string.Empty;
    }
}
