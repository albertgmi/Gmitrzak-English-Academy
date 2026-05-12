namespace inzBackend.Models.CourseModels
{
    public class UpdateCourseRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsHidden { get; set; }
    }
}
