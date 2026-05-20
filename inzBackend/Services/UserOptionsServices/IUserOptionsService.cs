using inzBackend.Models.UserOptionsModels;

namespace inzBackend.Services.UserOptionsServices
{
    public interface IUserOptionsService
    {
        List<UserOptionsDto> getAllOptions();
        UserOptionsDto getOptions(int userId);
        void updateOptions(int userId, UpdateUserOptionsRequest request);
    }
}
