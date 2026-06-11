namespace inzBackend.Models.ExaminationModels
{
    public class ExaminationSentenceDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
