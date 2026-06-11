namespace inzBackend.Models.AdminLearningModels
{
    public class LessonDailyFlashcardsDto
    {
        public DateOnly Date { get; set; }
        public int CardsStudied { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}
