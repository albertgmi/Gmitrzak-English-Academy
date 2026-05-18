using System.ComponentModel.DataAnnotations;

namespace inzBackend.Models.GlobalVocabularyModels
{
    public class VocabularyAddingRequest
    {
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
