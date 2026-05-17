namespace inzBackend.Models.DashboardModels
{
    public class StudentDashboardDto
    {
        public string Username { get; set; } = string.Empty;
        public int TotalActivityPoints { get; set; }
        public int FlashcardsDueToday { get; set; }
        public int FlashcardsStudiedToday { get; set; }
        public List<UpcomingAssignmentDto> ActiveAssignments { get; set; } = [];
        public List<UpcomingModuleDto> UpcomingModules { get; set; } = [];
        public bool LastWeekCriteriaMet { get; set; }
        public int CurrentStreak { get; set; }
    }
}
