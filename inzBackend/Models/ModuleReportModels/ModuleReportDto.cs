namespace inzBackend.Models.ModuleReportModels
{
    public class ModuleReportDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string StudentUsername { get; set; } = string.Empty;
        public DateOnly GeneratedDate { get; set; }
        public int TotalSentences { get; set; }
        public int CorrectCount { get; set; }
        public int PartialCount { get; set; }
        public int IncorrectCount { get; set; }
        public List<ModuleReportItemDto> Items { get; set; } = [];
    }
}
