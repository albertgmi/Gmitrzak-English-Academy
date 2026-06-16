namespace inzBackend.Models.AiPronunciationModels
{
    public class PronunciationAttemptDto
    {
        public int Id { get; set; }
        public string TranscribedText { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
