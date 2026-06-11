namespace inzBackend.Models.EssayModels
{
    public class EssayModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string EssayPrompt { get; set; } = string.Empty;
        public UserEssayDto? ExistingEssay { get; set; }
    }
}
