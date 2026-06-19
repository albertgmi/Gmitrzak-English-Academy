namespace inzBackend.Models.MatrixAssignmentModels
{
    public class ModuleUnlockDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleDescription { get; set; } = string.Empty;
        public int WeekNumber { get; set; }
        public int DayOfWeek { get; set; }
        public DateOnly UnlockDate { get; set; }
        public bool IsUnlocked { get; set; }
        public bool IsCompleted { get; set; }
        public int MatrixModuleId { get; set; }
        public DateOnly Deadline { get; set; }
    }
}
