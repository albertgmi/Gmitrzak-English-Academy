namespace inzBackend.Models.StudentLearningModels.MemoryModels
{
    public class MemoryDto
    {
        public int Id { get; set; }
        public string OptionA { get; set; } = string.Empty;
        public string? OptionB { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Category { get; set; }
    }
}
