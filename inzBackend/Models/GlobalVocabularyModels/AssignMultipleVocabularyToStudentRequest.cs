namespace inzBackend.Models.GlobalVocabularyModels
{
    public class AssignMultipleVocabularyToStudentRequest
    {
        public int StudentUserId { get; set; }
        public List<int> VocabularyIds { get; set; } = new();
    }
}
