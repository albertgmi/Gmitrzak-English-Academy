using inzBackend.Entities.Base;

namespace inzBackend.Entities.Curriculum
{
    public class Course : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsHidden { get; set; }
        public IEnumerable<CourseMatrix> CourseMatrices { get; set; } = new List<CourseMatrix>();
        public IEnumerable<ProgramCourse> ProgramCourses { get; set; } = new List<ProgramCourse>();

    }
}
