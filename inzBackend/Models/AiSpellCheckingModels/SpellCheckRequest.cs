namespace inzBackend.Models.AiSpellCheckingModels
{
    public class SpellCheckRequest
    {
        public string Text { get; set; } = string.Empty;
        public string? Language { get; set; }
    }
}
