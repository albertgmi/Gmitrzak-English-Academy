using inzBackend.Models.ProfileModels;
using inzBackend.Services.ProfileServices;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles ="Admin,User")]
        public ProfileDto GetProfile(int userId)
        {
            return _profileService.GetProfile(userId);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public void UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            _profileService.UpdateProfile(userId, request);
        }

        [HttpPost("{userId}/avatar")]
        [Authorize(Roles = "Admin")]
        public async Task<string> UploadAvatar(int userId, IFormFile file)
        {
            return await _profileService.UploadAvatar(userId, file);
        }
    }
}
