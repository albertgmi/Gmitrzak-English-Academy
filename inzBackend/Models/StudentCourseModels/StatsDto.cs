namespace inzBackend.Models.StudentCourseModels
{
    public class StatsDto
    {
        public List<DailyActivityDto> DailyActivity { get; set; } = [];
        public List<DailyFlashcardsDto> DailyFlashcards { get; set; } = [];
        public List<GradeDto> GradeHistory { get; set; } = [];
        public CategoryBreakdownDto CategoryBreakdown { get; set; } = new();
    }
}
