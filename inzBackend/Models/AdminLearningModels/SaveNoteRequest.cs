namespace inzBackend.Models.AdminLearningModels
{
    public class SaveNoteRequest
    {
        public int StudentUserId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
