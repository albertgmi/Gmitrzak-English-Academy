namespace inzBackend.Models.StudentLearningModels.AlphabetModels
{
    public class AlphabetAttemptDto
    {
        public int Id { get; set; }
        public string ProblemLetters { get; set; } = string.Empty;
        public string Feedback { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
