namespace inzBackend.Models.AdminLearningModels
{
    public class PronunciationTestItemDto
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateOnly? MarkedCorrectAt { get; set; }
        public int? DaysUntilRefresh { get; set; }
    }
}
