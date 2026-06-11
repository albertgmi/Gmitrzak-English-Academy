namespace inzBackend.Models.StudentCourseModels
{
    public class StudentModuleDto
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Order { get; set; }
        public int WeekNumber { get; set; }
        public int DayOfWeek { get; set; }
        public DateOnly UnlockDate { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOverdue { get; set; }
        public string Url { get; set; } = string.Empty;
        public int ActivityDaysCount { get; set; }
        public int ActivityDaysRequired { get; set; }
        public bool CanComplete { get; set; }
        public string? CompletionBlockReason { get; set; }
        public string? PresentationUrl { get; set; }
        public string? PresentationText { get; set; }
        public DateOnly Deadline { get; set; }
    }
}
