namespace inzBackend.Models.ModuleReportModels
{
    public class CompletedSentenceModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public DateOnly CompletedDate { get; set; }
        public bool IsFromMatrix { get; set; }
        public string? MatrixName { get; set; }
        public int TotalSentences { get; set; }
        public int AnsweredCount { get; set; }
    }
}
