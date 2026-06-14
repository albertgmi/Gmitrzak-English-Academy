using inzBackend.Entities.Base;

namespace inzBackend.Entities.Curriculum
{
    public class CourseMatrix : BaseEntity
    {
        public int CourseId { get; set; }
        public int MatrixId { get; set; }
        public int Order { get; set; }
        public Matrix Matrix { get; set; }
        public Course Course { get; set; }
    }
}
