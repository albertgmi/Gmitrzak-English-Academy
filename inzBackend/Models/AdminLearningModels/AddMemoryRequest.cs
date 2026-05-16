namespace inzBackend.Models.AdminLearningModels
{
    public class AddMemoryRequest
    {
        public int StudentUserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
