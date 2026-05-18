namespace inzBackend.Models.TheaterItemsModels
{
    public class UpdateTheaterItemRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string Level { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
