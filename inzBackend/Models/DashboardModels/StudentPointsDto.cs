namespace inzBackend.Models.DashboardModels
{
    public class StudentPointsDto
    {
        public string Username { get; set; } = string.Empty;
        public int TotalPoints { get; set; }
        public int ThisWeek { get; set; }
    }
}
