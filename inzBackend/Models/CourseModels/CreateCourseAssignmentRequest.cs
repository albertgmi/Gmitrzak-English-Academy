namespace inzBackend.Models.CourseModels
{
    public class CreateCourseAssignmentRequest
    {
        public int CourseId { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public List<int> UserIds { get; set; } = new();
    }
}
