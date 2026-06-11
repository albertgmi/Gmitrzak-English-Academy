namespace inzBackend.Models.AIAnswerCheckingModels
{
    public class AnswerResultDto
    {
        public int Id { get; set; }
        public string Polish { get; set; } = string.Empty;
        public string ExpectedTranslation { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string AiResult { get; set; } = string.Empty;
        public string AiExplanation { get; set; } = string.Empty;
        public string? TeacherOverride { get; set; }
        public string? TeacherExplanation { get; set; }
        public bool TeacherReviewed { get; set; }
        public string FinalResult => TeacherOverride ?? AiResult;
    }
}
