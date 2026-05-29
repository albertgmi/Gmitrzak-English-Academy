using inzBackend.Models.UserModels;
using inzBackend.Models;

namespace inzBackend.Services.UserServices
{
    public interface IUserService
    {
        AppUserDto registerUser(RegisterUserRequest request);
        string login(LoginUserRequest request);
        List<AppUserDto> getAllUsers();
        List<AppUserDto> getAllInactiveUsers();
        void updateUser(UpdateUserRequest request, int userId);
        void deleteUser(int userId);
        void deleteManyUsers(List<int> userIds);
        AppUserDto getUserById();
    }
}
