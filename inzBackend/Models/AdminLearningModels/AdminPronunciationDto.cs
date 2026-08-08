namespace inzBackend.Models.AdminLearningModels
{
    public class AdminPronunciationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsInCurrentSession { get; set; }
        public DateOnly? MarkedCorrectAt { get; set; }
    }
}
