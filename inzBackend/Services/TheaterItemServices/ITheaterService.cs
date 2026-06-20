using inzBackend.Models.TheaterItemsModels;

namespace inzBackend.Services.TheaterItemServices
{
    public interface ITheaterService
    {
        List<TheaterItemDto> GetAll();
        List<RepertoireItemDto> GetRepertoire();
        TheaterItemDto GetById(int id);
        TheaterItemDto Create(CreateTheaterItemRequest request);
        void Update(int id, UpdateTheaterItemRequest request);
        void Delete(int id);
        void ToggleActive(int id);
    }
}
