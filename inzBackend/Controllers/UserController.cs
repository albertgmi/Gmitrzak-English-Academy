using inzBackend.Models.UserModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        public ActionResult<List<AppUserDto>> getAllUsers()
        {
            return _userService.getAllUsers();
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
