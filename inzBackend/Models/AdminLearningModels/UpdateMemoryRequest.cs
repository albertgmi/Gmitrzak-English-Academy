namespace inzBackend.Models.AdminLearningModels
{
    public class UpdateMemoryRequest
    {
        public string Content { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string? OptionB { get; set; }
        public string? Notes { get; set; }
        public string? Category { get; set; }
    }
}
