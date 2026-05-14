namespace inzBackend.Models.StudentLearningModels.SentenceModels
{
    public class SentenceDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
