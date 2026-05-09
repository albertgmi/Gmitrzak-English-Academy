namespace inzBackend.Models
{
    public class ProgramCourse : BaseEntity
    {
        public int ProgramId { get; set; }
        public int CourseId { get; set; }
        public int Order { get; set; }
    }
}
