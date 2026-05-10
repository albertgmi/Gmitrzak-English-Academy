namespace inzBackend.Models
{
    public class MatrixModule : BaseEntity
    {
        public int MatrixId { get; set; }
        public int ModuleId { get; set; }
        public int Order { get; set; }
    }
}
