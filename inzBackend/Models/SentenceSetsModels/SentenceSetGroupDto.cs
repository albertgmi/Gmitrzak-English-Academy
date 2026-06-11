namespace inzBackend.Models.SentenceSetsModels
{
    public class SentenceSetGroupDto
    {
        public string GroupName { get; set; } = string.Empty;
        public List<SentenceSetDto> Sets { get; set; } = [];
    }
}
