namespace inzBackend.Models
{
    public class PronunciationEntry : AuditableEntity
    {
        public int UserId { get; set; }
        public string Word { get; set; } = string.Empty;
        public bool IsChecked { get; set; }
        public int SortOrder { get; set; }
    }
}
