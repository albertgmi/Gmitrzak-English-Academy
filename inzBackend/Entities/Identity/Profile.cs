using inzBackend.Entities.Base;
using inzBackend.Enums;

namespace inzBackend.Entities.Identity
{
    public class Profile : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public string? AvatarUrl { get; set; }
        public EnglishLevel? EnglishLevel { get; set; }
        public int? CurrentSemester { get; set; }

        public bool Semester1 { get; set; }
        public bool Semester2 { get; set; }
        public bool Semester3 { get; set; }
        public bool Semester4 { get; set; }
        public bool Semester5 { get; set; }
        public bool Semester6 { get; set; }
        public bool Semester7 { get; set; }
        public bool Semester8 { get; set; }
        public bool Semester9 { get; set; }
        public bool Semester10 { get; set; }
        public bool Semester11 { get; set; }
        public bool Semester12 { get; set; }
        public bool Semester13 { get; set; }
        public bool Semester14 { get; set; }
        public bool Semester15 { get; set; }
        public bool Semester16 { get; set; }
        public bool Semester17 { get; set; }
        public bool Semester18 { get; set; }
        public bool Semester19 { get; set; }
        public bool Semester20 { get; set; }
    }
}
