namespace inzBackend.Models.ModuleModels
{
    public class CreateModuleRequest
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public bool? IsHidden { get; set; }
        public int? TheaterItemId { get; set; }
    }
}
