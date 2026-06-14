using inzBackend.Entities.Base;

namespace inzBackend.Entities.Curriculum
{
    public class ProgramCourse : BaseEntity
    {
        public int ProgramId { get; set; }
        public virtual Program Program { get; set; }

        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public int Order { get; set; }
    }
}
