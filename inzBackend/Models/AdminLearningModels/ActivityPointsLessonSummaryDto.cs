namespace inzBackend.Models.AdminLearningModels
{
    public class ActivityPointsLessonSummaryDto
    {
        public int TotalAllTime { get; set; }
        public int TotalThisWeek { get; set; }
        public int TotalLastWeek { get; set; }
        public List<ActivityPointLessonDto> History { get; set; } = [];
    }
}
