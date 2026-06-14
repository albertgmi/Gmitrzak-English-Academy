using inzBackend.Entities.Base;
using inzBackend.Entities.Resources;

namespace inzBackend.Entities.Curriculum
{
    public class Module : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool? IsHidden { get; set; }
        public string Category { get; set; } = string.Empty;
        public ICollection<MatrixModule> MatrixModules { get; set; } = new List<MatrixModule>();
        public int? TheaterItemId { get; set; }
        public string? EssayPrompt { get; set; }
        public TheaterItem? TheaterItem { get; set; }
        public ModulePresentation? Presentation { get; set; }
    }
}
