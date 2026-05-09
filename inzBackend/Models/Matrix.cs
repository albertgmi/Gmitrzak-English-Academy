namespace inzBackend.Models
{
    public class Matrix : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public int RefreshIntervalDays { get; set; }
        public IEnumerable<MatrixModule> MatrixModules { get; set; } = new List<MatrixModule>();
    }
}
