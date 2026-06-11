using inzBackend.Models.ModuleReportModels;

namespace inzBackend.Services.ReportServices
{
    public interface IModuleReportExportService
    {
        byte[] generateRangePdf(DateRangeReportDto report);
        byte[] generateRangeDocx(DateRangeReportDto report);
        byte[] generateActiveStudentsZipReport(DateOnly dateFrom, DateOnly dateTo);
    }
}
