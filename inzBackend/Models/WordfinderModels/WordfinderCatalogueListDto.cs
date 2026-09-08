using inzBackend.Enums;

namespace inzBackend.Models.WordfinderModels
{
    public class WordfinderCatalogueListDto
    {
        public int Id { get; set; }
        public int StudentUserId { get; set; }
        public string StudentUsername { get; set; } = string.Empty;
        public string StudentInitials { get; set; } = string.Empty;
        public string? StudentAvatarUrl { get; set; }
        public string Name { get; set; } = string.Empty;
        public WordfinderCatalogueStatus Status { get; set; }
        public string StatusName => Status.ToString();
        public int EntryCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public DateTimeOffset? ReviewedAt { get; set; }
    }
}
