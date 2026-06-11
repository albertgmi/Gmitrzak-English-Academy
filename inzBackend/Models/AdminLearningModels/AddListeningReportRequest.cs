namespace inzBackend.Models.AdminLearningModels
{
    public class AddListeningReportRequest
    {
        public int StudentUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public int EpisodeCount { get; set; } = 1;
    }
}
