using inzBackend.Models;

namespace inzBackend.Entities
{
    public class ModulePresentation : BaseEntity
    {
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public string? Url { get; set; }
        public string? Text { get; set; }
    }
}
