namespace inzBackend.Models.DashboardModels
{
    public class UpcomingAssignmentDto
    {
        public int Id { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public bool IsOverdue { get; set; }
    }
}
