using inzBackend.Models.ProfileModels;

namespace inzBackend.Services.ProfileServices
{
    public interface IProfileService
    {
        ProfileDto getProfile(int userId);
        void updateProfile(int userId, UpdateProfileRequest request);
        Task<string> uploadAvatar(int userId, IFormFile file);
    }
}
