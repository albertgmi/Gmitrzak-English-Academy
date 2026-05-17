namespace inzBackend.Models.AdminLearningModels
{
    public class LessonStatsDto
    {
        public List<LessonDailyActivityDto> DailyActivity { get; set; } = [];
        public List<LessonDailyFlashcardsDto> DailyFlashcards { get; set; } = [];
        public List<LessonGradeDto> GradeHistory { get; set; } = [];
        public LessonCategoryBreakdownDto CategoryBreakdown { get; set; } = new();
    }
}
