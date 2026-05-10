using inzBackend.Models.UserModels;
using inzBackend.Models;

namespace inzBackend.Services.UserServices
{
    public interface IUserService
    {
        AppUser registerUser(RegisterUserRequest request);
        string Login(LoginUserRequest request);
    }
}
