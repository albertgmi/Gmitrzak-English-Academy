namespace inzBackend.Models.ModuleReportModels
{
    public class ModuleReportItemDto
    {
        public int Order { get; set; }
        public string Polish { get; set; } = string.Empty;
        public string ExpectedTranslation { get; set; } = string.Empty;
        public string StudentAnswer { get; set; } = string.Empty;
        public string AiResult { get; set; } = string.Empty;
        public string AiExplanation { get; set; } = string.Empty;
        public string? TeacherOverride { get; set; }
        public string? TeacherExplanation { get; set; }
        public string FinalResult { get; set; } = string.Empty;
    }
}
