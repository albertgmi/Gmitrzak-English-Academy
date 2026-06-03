namespace inzBackend.Models.StudentLearningModels.PronunciationEntryModels
{
    public class PronunciationEntryDto
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsInCurrentSession { get; set; }
    }
}
