using inzBackend.Models.MatrixModels;
using inzBackend.Models.ProgramModels;

namespace inzBackend.Models.CourseModels
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
        public List<MatrixDto> MatrixDtos { get; set; }
        public List<ProgramSimpleDto> Programs { get; set; }
    }
}
