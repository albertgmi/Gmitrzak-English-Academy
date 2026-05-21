namespace inzBackend.Models.ModuleSentenceModels
{
    public class ModuleSentenceSessionDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public List<ModuleSentenceItemDto> Sentences { get; set; } = [];
    }
}
