using inzBackend.Entities.Base;
using inzBackend.Entities.Curriculum;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Assignments
{
    public class UserEssay : AuditableEntity
    {
        public int UserId { get; set; }
        public AppUser User { get; set; } = null!;
        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public string? AdminContent { get; set; }
        public bool IsSubmitted { get; set; } = false;
        public bool IsReviewed { get; set; } = false;
        public DateOnly? SubmittedDate { get; set; }
        public DateOnly? ReviewedDate { get; set; }
    }
}
