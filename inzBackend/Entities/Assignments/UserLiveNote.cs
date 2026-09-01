using System;
using inzBackend.Entities.Identity;

namespace inzBackend.Entities.Assignments
{
    public class UserLiveNote
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public AppUser Student { get; set; } = null!;

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public int CreatedById { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;

        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? LastModifiedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
