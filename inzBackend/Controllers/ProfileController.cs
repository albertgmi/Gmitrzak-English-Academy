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
        public ProfileDto getProfile([FromRoute] int userId)
        {
            return _profileService.getProfile(userId);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public void updateProfile([FromRoute] int userId, [FromBody] UpdateProfileRequest request)
        {
            _profileService.updateProfile(userId, request);
        }

        [HttpPost("{userId}/avatar")]
        [Authorize(Roles = "Admin,User")]
        public async Task<string> uploadAvatar([FromRoute] int userId, IFormFile file)
        {
            return await _profileService.uploadAvatar(userId, file);
        }
    }
}
