using System.Text.Json.Serialization;

namespace inzBackend.Models.CatalogueModels
{
    public class TranslationResult
    {
        [JsonPropertyName("translations")]
        public List<string> Translations { get; set; } = new();
    }
}
