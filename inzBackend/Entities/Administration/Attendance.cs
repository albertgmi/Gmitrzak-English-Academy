using inzBackend.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using inzBackend.Entities.Base;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Administration
{
    public class Attendance : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public AttendanceType Type { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
