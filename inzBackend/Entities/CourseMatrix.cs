namespace inzBackend.Models
{
    public class CourseMatrix : BaseEntity
    {
        public int CourseId { get; set; }
        public int MatrixId { get; set; }
        public int Order { get; set; }
        public Matrix Matrix { get; set; }
    }
}
