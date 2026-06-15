namespace inzBackend.Models.AiSpellCheckingModels
{
    public class SpellCheckResult
    {
        public bool HasError { get; set; }
        public string? Corrected { get; set; }
        public string? Reason { get; set; }
    }
}
