using inzBackend.Models.UserModels;
using inzBackend.Models;

namespace inzBackend.Services.UserServices
{
    public interface IUserService
    {
        AppUser registerUser(RegisterUserRequest request);
        string login(LoginUserRequest request);
        List<AppUserDto> getAllUsers(bool? active);
        void updateUser(UpdateUserRequest request, int userId);
        void deleteUser(int userId);
        void deleteManyUsers(List<int> userIds);
        AppUserDto getUserById();
    }
}
