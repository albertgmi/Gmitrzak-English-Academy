namespace inzBackend.Models.DashboardModels
{
    public class UpcomingModuleDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public string MatrixName { get; set; } = string.Empty;
        public DateOnly UnlockDate { get; set; }
        public bool IsUnlocked { get; set; }
    }
}
