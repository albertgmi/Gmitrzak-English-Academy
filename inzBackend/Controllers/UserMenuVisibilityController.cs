using inzBackend.Models.UserModels;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/user-menu-visibility")]
    [ApiController]
    public class UserMenuVisibilityController : ControllerBase
    {
        private readonly IUserMenuVisibilityService _menuVisibilityService;

        public UserMenuVisibilityController(IUserMenuVisibilityService menuVisibilityService)
        {
            _menuVisibilityService = menuVisibilityService;
        }

        [HttpGet("students")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AppUserDto>> GetStudentsForMenuVisibility()
        {
            return Ok(_menuVisibilityService.GetStudentsForMenuVisibility());
        }

        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<string>> GetHiddenMenuItemsForUser([FromRoute] int userId)
        {
            var hiddenItems = _menuVisibilityService.GetHiddenMenuItemsForUser(userId);
            return Ok(hiddenItems);
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateUserMenuVisibility([FromRoute] int userId, [FromBody] UpdateUserMenuVisibilityRequest request)
        {
            _menuVisibilityService.UpdateUserMenuVisibility(userId, request.HiddenMenuItemKeys);
            return Ok();
        }

        [HttpGet("my")]
        [Authorize]
        public ActionResult<List<string>> GetMyHiddenMenuItems()
        {
            var hiddenItems = _menuVisibilityService.GetMyHiddenMenuItems();
            return Ok(hiddenItems);
        }
    }
}
