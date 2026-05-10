namespace inzBackend.Models
{
    public class Module : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
    }
}
