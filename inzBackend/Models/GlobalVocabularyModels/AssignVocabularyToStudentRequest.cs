namespace inzBackend.Models.GlobalVocabularyModels
{
    public class AssignVocabularyToStudentRequest
    {
        public int VocabularyId { get; set; }
        public int StudentUserId { get; set; }
    }
}
