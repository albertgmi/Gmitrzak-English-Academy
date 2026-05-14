using inzBackend.Models.UserModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inzBackend.Controllers
{
    [Route("api/user")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        public ActionResult<List<AppUserDto>> getAllUsers([FromQuery] bool? active)
        {
            return Ok(_userService.getAllUsers(active));
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult<AppUserDto> getUserById()
        {
            return Ok(_userService.getUserById());
        }

        [HttpPut("update/{userId}")]
        public ActionResult updateUser([FromBody] UpdateUserRequest request, [FromRoute] int userId)
        {
            _userService.updateUser(request, userId);
            return Ok();
        }

        [HttpDelete("delete/{userId}")]
        public ActionResult deleteUser([FromRoute] int userId)
        {
            _userService.deleteUser(userId);
            return Ok();
        }

        [HttpDelete("delete")]
        public ActionResult deleteManyUsers([FromQuery] List<int> userIds)
        {
            _userService.deleteManyUsers(userIds);
            return Ok();
        }
    }
}
