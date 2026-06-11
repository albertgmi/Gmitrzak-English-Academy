namespace inzBackend.Models.StudentCourseModels
{
    public class GradeDto
    {
        public int Id { get; set; }
        public DateOnly GradeDate { get; set; }
        public decimal Percentage { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
