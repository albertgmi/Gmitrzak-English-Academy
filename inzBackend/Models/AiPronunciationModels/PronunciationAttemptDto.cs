namespace inzBackend.Models.AiPronunciationModels
{
    public class PronunciationAttemptDto
    {
        public int Id { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public int Score { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
