namespace inzBackend.Models.StudentCourseModels
{
    public class StudentAssignmentDto
    {
        public int MatrixId { get; set; }
        public string MatrixName { get; set; }
        public DateOnly StartDate { get; set; }
        public int RefreshIntervalDays { get; set; }
        public bool HasAnyUnlocked => Modules.Any(m => m.IsUnlocked);
        public List<StudentModuleDto> Modules { get; set; } = new();
    }
}
