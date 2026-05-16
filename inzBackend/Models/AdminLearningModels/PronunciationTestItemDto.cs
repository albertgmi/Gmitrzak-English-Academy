namespace inzBackend.Models.AdminLearningModels
{
    public class PronunciationTestItemDto
    {
        public int Id { get; set; }
        public string Word { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public int SortOrder { get; set; }
    }
}
