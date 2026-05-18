using inzBackend.Models.StreamEntryModels;

namespace inzBackend.Services.StreamEntryServices
{
    public interface IStreamService
    {
        List<StreamEntryListDto> getAll(int? userId);
        void create(CreateStreamEntryRequest request);
        void delete(int id);
        void deleteMultiple(List<int> ids);
    }
}
