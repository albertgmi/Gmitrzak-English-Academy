using inzBackend.Models.UserModels;
using inzBackend.Models;

namespace inzBackend.Services.UserServices
{
    public interface IUserService
    {
        AppUserDto RegisterUser(RegisterUserRequest request);
        string Login(LoginUserRequest request);
        List<AppUserDto> GetAllUsers();
        List<AppUserDto> GetAllInactiveUsers();
        void UpdateUser(UpdateUserRequest request, int userId);
        void DeleteUser(int userId);
        void DeleteManyUsers(List<int> userIds);
        AppUserDto GetUserById();
    }
}
