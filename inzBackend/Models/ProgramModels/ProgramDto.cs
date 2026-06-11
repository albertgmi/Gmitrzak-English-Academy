using inzBackend.Models.CourseModels;

namespace inzBackend.Models.ProgramModels
{
    public class ProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
        public List<CourseDto> CourseDtos { get; set; }
    }
}
