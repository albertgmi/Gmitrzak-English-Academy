namespace inzBackend.Models.AdminLearningModels
{
    public class AddSentenceRequest
    {
        public int StudentUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
