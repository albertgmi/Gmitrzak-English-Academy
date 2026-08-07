namespace inzBackend.Models.StudentLearningModels.AlphabetModels
{
    public class AlphabetHistoryItemDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateOnly WeekStartDate { get; set; }
        public DateOnly? MarkedCorrectAt { get; set; }
        public int AttemptCount { get; set; }
        public List<AlphabetAttemptDto> Attempts { get; set; } = new();
    }
}
