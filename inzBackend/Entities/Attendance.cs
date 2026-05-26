using inzBackend.Enums;
using inzBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace inzBackend.Entities
{
    public class Attendance : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public AttendanceType Type { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
