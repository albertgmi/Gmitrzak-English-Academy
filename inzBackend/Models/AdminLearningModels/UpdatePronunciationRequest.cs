namespace inzBackend.Models.AdminLearningModels
{
    public class UpdatePronunciationRequest
    {
        public string Word { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsInCurrentSession { get; set; }
    }
}
