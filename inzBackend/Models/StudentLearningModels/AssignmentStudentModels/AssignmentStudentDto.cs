namespace inzBackend.Models.StudentLearningModels.AssignmentStudentModels
{
    public class AssignmentStudentDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleDescription { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsFromMatrix { get; set; }
        public string MatrixName { get; set; } = string.Empty;
        public bool HasDeadline { get; set; }
    }
}
