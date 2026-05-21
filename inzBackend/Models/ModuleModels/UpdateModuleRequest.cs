namespace inzBackend.Models.ModuleModels
{
    public class UpdateModuleRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool? IsHidden { get; set; }
    }
}
