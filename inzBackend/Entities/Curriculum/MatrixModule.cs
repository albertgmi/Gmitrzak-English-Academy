using inzBackend.Entities.Base;

namespace inzBackend.Entities.Curriculum
{
    public class MatrixModule : AuditableEntity
    {
        public int MatrixId { get; set; }
        public Matrix Matrix { get; set; } = null!;
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public int WeekNumber { get; set; }
        public int DayOfWeek { get; set; }
        public int Order { get; set; }
    }
}
