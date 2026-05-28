namespace inzBackend.Models.ModuleModels
{
    public class CreateModuleRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsHidden { get; set; }
        public string Category { get; set; } = "General";
        public int? TheaterItemId { get; set; }
        public string? PresentationUrl { get; set; }
        public string? PresentationText { get; set; }
    }
}
