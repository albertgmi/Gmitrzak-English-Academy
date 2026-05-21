using inzBackend.Models.ModuleReportModels;

namespace inzBackend.Services.ReportServices
{
    public interface IModuleReportExportService
    {
        byte[] GeneratePdf(ModuleReportDto report);
        byte[] GenerateDocx(ModuleReportDto report);
    }
}
