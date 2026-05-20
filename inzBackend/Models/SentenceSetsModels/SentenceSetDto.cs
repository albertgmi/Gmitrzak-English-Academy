namespace inzBackend.Models.SentenceSetsModels
{
    public class SentenceSetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int Order { get; set; }
        public int ItemCount { get; set; }
        public List<SentenceSetItemDto> Items { get; set; } = [];
    }
}
