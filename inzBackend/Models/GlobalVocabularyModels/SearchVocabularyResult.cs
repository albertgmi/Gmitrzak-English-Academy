namespace inzBackend.Models.GlobalVocabularyModels
{
    public class SearchVocabularyResult
    {
        public int Id { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool ExistsInGlobal { get; set; }
        public bool AlreadyAssignedToStudent { get; set; }
    }
}
