namespace inzBackend.Models.StudentLearningModels.AlphabetModels
{
    public class AlphabetTestItemDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateOnly? MarkedCorrectAt { get; set; }
    }
}
