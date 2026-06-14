using inzBackend.Entities.Base;

namespace inzBackend.Entities.Curriculum
{
    public class ModulePresentation : BaseEntity
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public string? Url { get; set; }
        public string? Text { get; set; }
    }
}
