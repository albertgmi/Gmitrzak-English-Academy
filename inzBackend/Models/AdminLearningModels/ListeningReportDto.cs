namespace inzBackend.Models.AdminLearningModels
{
    public class ListeningReportDto
    {
        public int Id { get; set; }
        public DateOnly ReportDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public int EpisodeCount { get; set; }
    }
}
