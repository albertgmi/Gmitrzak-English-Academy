namespace inzBackend.Models.GlobalVocabularyModels
{
    public class VocabularyForAdminDto
    {
        public int Id { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Back { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<int> AssignedStudentIds { get; set; } = new();
    }
}
