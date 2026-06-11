namespace inzBackend.Models.AdminLearningModels
{
    public class TeacherNoteDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateOnly NoteDate { get; set; }
    }
}
