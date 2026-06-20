namespace inzBackend.Models.ModuleModels
{
    public class ActivityStatus
    {
        public int Streak { get; set; }
        public int Required { get; set; }
        public bool CanComplete { get; set; }
        public string? BlockReason { get; set; }
    }
}
