using inzBackend.Models.MatrixModels;

namespace inzBackend.Models.CourseModels
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHidden { get; set; }
        public List<MatrixDto> MatrixDtos { get; set; }
    }
}
