namespace inzBackend.Models
{
    public class Matrix : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RefreshIntervalDays { get; set; }
        public bool? IsHidden { get; set; }
        public IEnumerable<MatrixModule> MatrixModules { get; set; } = new List<MatrixModule>();
        public IEnumerable<CourseMatrix> CourseMatrices { get; set; } = new List<CourseMatrix>();
    }
}
