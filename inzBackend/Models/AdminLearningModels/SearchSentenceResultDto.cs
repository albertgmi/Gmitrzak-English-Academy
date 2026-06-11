namespace inzBackend.Models.AdminLearningModels
{
    public class SearchSentenceResultDto
    {
        public int? Id { get; set; }
        public string Polish { get; set; } = string.Empty;
        public string EnglishTranslation { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool ExistsInGlobal { get; set; }
        public bool AlreadyAssignedToStudent { get; set; }
    }
}
