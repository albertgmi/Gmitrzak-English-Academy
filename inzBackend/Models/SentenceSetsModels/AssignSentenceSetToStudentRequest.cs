namespace inzBackend.Models.SentenceSetsModels
{
    public class AssignSentenceSetToStudentRequest
    {
        public int UserId { get; set; }
        public int SentenceSetId { get; set; }
        public string DueDate { get; set; } = string.Empty;
    }
}
