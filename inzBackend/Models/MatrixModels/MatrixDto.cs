using inzBackend.Models.CourseModels;
using inzBackend.Models.ModuleModels;

namespace inzBackend.Models.MatrixModels
{
    public class MatrixDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int RefreshIntervalDays { get; set; }
        public bool IsHidden { get; set; }
        public List<ModuleSimpleDto> Modules { get; set; }
        public List<CourseSimpleDto> Courses { get; set; }
    }
}
