using inzBackend.Models.UserModels;
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
        public ActionResult<List<AppUserDto>> GetAllUsers()
        {
            return Ok(_userService.GetAllUsers());
        }

        [HttpGet("users/inactive")]
        public ActionResult<List<AppUserDto>> GetAllInactiveUsers()
        {
            return Ok(_userService.GetAllInactiveUsers());
        }
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult<AppUserDto> GetUserById()
        {
            return Ok(_userService.GetUserById());
        }

        [HttpPut("update/{userId}")]
        public ActionResult UpdateUser([FromBody] UpdateUserRequest request, [FromRoute] int userId)
        {
            _userService.UpdateUser(request, userId);
            return Ok();
        }

        [HttpDelete("delete/{userId}")]
        public ActionResult DeleteUser([FromRoute] int userId)
        {
            _userService.DeleteUser(userId);
            return Ok();
        }

        [HttpDelete("delete")]
        public ActionResult DeleteManyUsers([FromQuery] List<int> userIds)
        {
            _userService.DeleteManyUsers(userIds);
            return Ok();
        }
    }
}
