namespace inzBackend.Models.AdminLearningModels
{
    public class AddTranslationRequest
    {
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
