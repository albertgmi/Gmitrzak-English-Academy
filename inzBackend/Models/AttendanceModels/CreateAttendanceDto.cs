using System.ComponentModel.DataAnnotations;

namespace inzBackend.Models.AttendanceModels
{
    public class CreateAttendanceDto
    {
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Duration { get; set; }
    }
}
