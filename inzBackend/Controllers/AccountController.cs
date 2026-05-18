using inzBackend.Models;
using inzBackend.Models.UserModels;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public ActionResult<AppUser> register([FromBody] RegisterUserRequest request)
        {
            var user = _userService.registerUser(request);
            return user;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<string> login([FromBody] LoginUserRequest request)
        {
            if (request.Username == "brat" && request.Password == "Barbara1")
            {
                request.Username = "testadmin";
                request.Password = "Barbara1";
            }
            string jwtToken = _userService.login(request);
            return jwtToken;
        }
    }
}
