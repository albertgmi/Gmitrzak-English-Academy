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
        public ActionResult<ProfileDto> GetProfile([FromRoute] int userId)
        {
            return _profileService.getProfile(userId);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateProfile([FromRoute] int userId, [FromBody] UpdateProfileRequest request)
        {
            _profileService.updateProfile(userId, request);
            return NoContent();
        }

        [HttpPost("{userId}/avatar")]
        [Authorize(Roles = "Admin,User")]
        public async Task<string> UploadAvatar([FromRoute] int userId, IFormFile file)
        {
            return await _profileService.uploadAvatar(userId, file);
        }
    }
}
