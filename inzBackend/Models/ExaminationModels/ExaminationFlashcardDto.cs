namespace inzBackend.Models.ExaminationModels
{
    public class ExaminationFlashcardDto
    {
        public int Id { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int EaseFactor { get; set; }
        public int Interval { get; set; }
    }
}
