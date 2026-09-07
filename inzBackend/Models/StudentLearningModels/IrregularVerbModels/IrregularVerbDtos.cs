using inzBackend.Enums;

namespace inzBackend.Models.StudentLearningModels.IrregularVerbModels
{
    public class IrregularVerbDto
    {
        public int Id { get; set; }
        public string PolishTranslation { get; set; } = string.Empty;
        public string EnglishForms { get; set; } = string.Empty;
        public IrregularVerbLevel Level { get; set; }
        public int EaseFactor { get; set; }
        public int Interval { get; set; }
        public bool IsLeech { get; set; }
        public DateOnly NextReviewDate { get; set; }
    }

    public class ReviewIrregularVerbRequest
    {
        public string Quality { get; set; } = string.Empty;
        public int TimeSpentSeconds { get; set; }
    }

    public class LessonIrregularVerbSummaryDto
    {
        public int TotalCards { get; set; }
        public int DueCount { get; set; }
        public int StudiedTodayCount { get; set; }
        public int LeechCount { get; set; }
        public List<IrregularVerbDto> Leeches { get; set; } = new();
    }
}
