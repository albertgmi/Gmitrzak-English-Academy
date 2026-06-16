namespace inzBackend.Models.AiPronunciationModels
{
    public class PronunciationEvaluationJson
    {
        public int Score { get; set; }
        public string Result { get; set; } = "";
        public string Feedback { get; set; } = "";
    }
}
