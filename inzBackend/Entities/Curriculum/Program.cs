using inzBackend.Entities.Base;

namespace inzBackend.Entities.Curriculum
{
    public class Program : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool? IsHidden { get; set; }
        public IEnumerable<ProgramCourse> ProgramCourses { get; set; } = new List<ProgramCourse>();
    }
}
