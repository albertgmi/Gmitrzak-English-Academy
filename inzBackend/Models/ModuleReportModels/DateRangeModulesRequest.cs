namespace inzBackend.Models.ModuleReportModels
{
    public class DateRangeModulesRequest
    {
        public int StudentUserId { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
    }
}
