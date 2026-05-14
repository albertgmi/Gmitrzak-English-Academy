namespace inzBackend.Models.StudentLearningModels.PronunciationEntryModels
{
    public class PronunciationEntryDto
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public int SortOrder { get; set; }
    }
}
