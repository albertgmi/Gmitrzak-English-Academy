using inzBackend.Models.TheaterItemsModels;

namespace inzBackend.Services.TheaterItemServices
{
    public interface ITheaterService
    {
        List<TheaterItemDto> getAll();
        List<RepertoireItemDto> getRepertoire();
        TheaterItemDto getById(int id);
        TheaterItemDto create(CreateTheaterItemRequest request);
        void update(int id, UpdateTheaterItemRequest request);
        void delete(int id);
        void toggleActive(int id);
    }
}
