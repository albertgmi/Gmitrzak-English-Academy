namespace inzBackend.Models.AttendanceModels
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Duration { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
