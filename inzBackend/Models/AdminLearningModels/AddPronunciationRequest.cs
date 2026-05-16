namespace inzBackend.Models.AdminLearningModels
{
    public class AddPronunciationRequest
    {
        public int StudentUserId { get; set; }
        public string Word { get; set; } = string.Empty;
    }
}
