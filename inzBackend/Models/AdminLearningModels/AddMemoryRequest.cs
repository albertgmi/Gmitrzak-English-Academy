namespace inzBackend.Models.AdminLearningModels
{
    public class AddMemoryRequest
    {
        public int StudentUserId { get; set; }
        public string OptionA { get; set; } = string.Empty;
        public string? OptionB { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
    }
}
