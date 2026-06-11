namespace inzBackend.Models.EssayModels
{
    public class SubmitEssayRequest
    {
        public int ModuleId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
