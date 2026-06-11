namespace inzBackend.Models.StudentCourseModels
{
    public class ActivityPointsHistoryDto
    {
        public int TotalAllTime { get; set; }
        public int TotalThisWeek { get; set; }
        public int TotalLastWeek { get; set; }
        public List<ActivityPointDto> History { get; set; } = [];
    }
}
