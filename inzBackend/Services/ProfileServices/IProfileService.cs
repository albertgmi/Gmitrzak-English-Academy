using inzBackend.Models.ProfileModels;

namespace inzBackend.Services.ProfileServices
{
    public interface IProfileService
    {
        ProfileDto GetProfile(int userId);
        void UpdateProfile(int userId, UpdateProfileRequest request);
        Task<string> UploadAvatar(int userId, IFormFile file);
    }
}
