namespace inzBackend.Models.ModuleModels
{
    public class UpdateModuleRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsHidden { get; set; }
        public string? Category { get; set; }
        public int? TheaterItemId { get; set; }
        public string? PresentationUrl { get; set; }
        public string? PresentationText { get; set; }
        public string? EssayPrompt { get; set; }
    }
}
