namespace inzBackend.Models.StudentCourseModels
{
    public class ActivityPointDto
    {
        public int Id { get; set; }
        public DateOnly PointDate { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
