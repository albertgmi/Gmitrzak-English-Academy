using inzBackend.Models.MatrixModels;

namespace inzBackend.Models.ModuleModels
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public List<MatrixSimpleDto> Matrices { get; set; }
        public string? Url { get; set; }
    }
}
