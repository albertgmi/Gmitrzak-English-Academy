namespace inzBackend.Models.StudentLearningModels.AlphabetModels
{
    public class AlphabetEntryDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateOnly WeekStartDate { get; set; }
    }
}
