using inzBackend.Models.ModuleModels;

namespace inzBackend.Services.SectionActivityServices
{
    public interface ISectionActivityService
    {
        void LogActivity(LogActivityRequest request);
    }
}