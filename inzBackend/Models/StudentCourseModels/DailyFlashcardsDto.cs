namespace inzBackend.Models.StudentCourseModels
{
    public class DailyFlashcardsDto
    {
        public DateOnly Date { get; set; }
        public int CardsStudied { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}
