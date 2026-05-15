namespace inzBackend.Models.StudentLearningModels.FlashcardModels
{
    public class ReviewCardRequest
    {
        public string Quality { get; set; } = string.Empty;
        public int TimeSpentSeconds { get; set; }
    }
}
