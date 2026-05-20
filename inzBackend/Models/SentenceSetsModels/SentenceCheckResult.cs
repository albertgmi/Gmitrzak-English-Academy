using inzBackend.Enums;
using System.Text.Json.Serialization;

namespace inzBackend.Models.SentenceSetsModels
{
    public class SentenceCheckResult
    {
        [JsonPropertyName("result")]
        public EvaluationResult Result { get; set; }

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; }
        public SentenceCheckResult() { }
        public SentenceCheckResult(EvaluationResult result, string explanation)
        {
            Result = result;
            Explanation = explanation;
        }
    }
}
