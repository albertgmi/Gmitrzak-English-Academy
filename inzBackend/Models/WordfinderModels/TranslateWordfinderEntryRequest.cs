namespace inzBackend.Models.WordfinderModels
{
    public class TranslateWordfinderEntryRequest
    {
        public string FrontText { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = "Polish";
    }
}
