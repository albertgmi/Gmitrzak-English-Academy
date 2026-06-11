namespace inzBackend.Models.DashboardModels
{
    public class RecentGradeDto
    {
        public string Username { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public DateOnly GradeDate { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
