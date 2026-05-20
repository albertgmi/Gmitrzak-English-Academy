namespace inzBackend.Models.ModuleSentenceModels
{
    public class ModuleSentenceSessionDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public int AssignmentId { get; set; }
        public List<ModuleSentenceItemDto> Sentences { get; set; } = [];
    }
}
