using inzBackend.Models;
using inzBackend.Models.UserModels;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public ActionResult<AppUser> register([FromBody] RegisterUserRequest request)
        {
            var user = _userService.registerUser(request);
            return user;
        }

        [HttpPost("login")]
        public ActionResult<string> login([FromBody] LoginUserRequest request)
        {
            string jwtToken = _userService.Login(request);
            return jwtToken;
        }
    }
}
