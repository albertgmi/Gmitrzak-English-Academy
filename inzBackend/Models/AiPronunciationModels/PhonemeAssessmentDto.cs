namespace inzBackend.Models.AiPronunciationModels
{
    public class PhonemeAssessmentDto
    {
        public string Phoneme { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
