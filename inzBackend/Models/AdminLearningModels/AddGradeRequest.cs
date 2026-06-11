namespace inzBackend.Models.AdminLearningModels
{
    public class AddGradeRequest
    {
        public int StudentUserId { get; set; }
        public decimal Percentage { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
