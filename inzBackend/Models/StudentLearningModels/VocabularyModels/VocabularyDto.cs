namespace inzBackend.Models.StudentLearningModels.VocabularyModels
{
    public class VocabularyDto
    {
        public int Id { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Interval { get; set; }
        public bool IsLeech { get; set; }
        public DateOnly NextReviewDate { get; set; }
    }
}
