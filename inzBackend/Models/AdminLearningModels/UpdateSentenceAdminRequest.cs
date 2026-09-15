namespace inzBackend.Models.AdminLearningModels
{
    public class UpdateSentenceAdminRequest
    {
        public string Translation { get; set; } = string.Empty;
        public int? Interval { get; set; }
        public string? Content { get; set; }
        public string? Notes { get; set; }
    }
}
