namespace inzBackend.Models.ModuleReportModels
{
    public class DateRangeReportDto
    {
        public string StudentUsername { get; set; } = string.Empty;
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
        public DateOnly GeneratedDate { get; set; }
        public List<ModuleReportDto> Modules { get; set; } = [];
        public int TotalCorrect { get; set; }
        public int TotalPartial { get; set; }
        public int TotalIncorrect { get; set; }
        public int TotalSentences { get; set; }
    }
}
