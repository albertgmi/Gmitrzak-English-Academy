namespace inzBackend.Models.AdminLearningModels
{
    public class GradeListDto
    {
        public int Id { get; set; }
        public DateOnly GradeDate { get; set; }
        public decimal Percentage { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
