using inzBackend.Models.ProfileModels;
using inzBackend.Services.ProfileServices;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("{userId}")]
        public ProfileDto GetProfile(int userId)
        {
            return _profileService.GetProfile(userId);
        }

        [HttpPut("{userId}")]
        public void UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            _profileService.UpdateProfile(userId, request);
        }

        [HttpPost("{userId}/avatar")]
        public async Task<string> UploadAvatar(int userId, IFormFile file)
        {
            return await _profileService.UploadAvatar(userId, file);
        }
    }
}
