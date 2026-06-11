namespace inzBackend.Models.AdminLearningModels
{
    public class AssignFlashcardToStudentRequest
    {
        public int GlobalFlashcardId { get; set; }
        public int StudentUserId { get; set; }
    }
}
