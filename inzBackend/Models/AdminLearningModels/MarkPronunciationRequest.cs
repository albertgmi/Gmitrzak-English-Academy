namespace inzBackend.Models.AdminLearningModels
{
    public class MarkPronunciationRequest
    {
        public int EntryId { get; set; }
        public string Result { get; set; } = string.Empty;
    }
}
