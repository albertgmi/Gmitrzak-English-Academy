using inzBackend.Models.ModuleReportModels;

namespace inzBackend.Services.ReportServices
{
    public interface IModuleReportExportService
    {
        byte[] GenerateRangePdf(DateRangeReportDto report);
        byte[] GenerateRangeDocx(DateRangeReportDto report);
        byte[] GenerateActiveStudentsZipReport(DateOnly dateFrom, DateOnly dateTo);
    }
}
