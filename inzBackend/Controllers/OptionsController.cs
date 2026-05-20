using inzBackend.Models.UserOptionsModels;
using inzBackend.Services.UserOptionsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/options")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class OptionsController : ControllerBase
    {
        private readonly IUserOptionsService _service;
        public OptionsController(IUserOptionsService service) { _service = service; }

        [HttpGet]
        public List<UserOptionsDto> getAll()
        {
            return _service.getAllOptions();
        }

        [HttpGet("{userId}")]
        public UserOptionsDto get(int userId)
        {
            return _service.getOptions(userId);
        }

        [HttpPut("{userId}")]
        public ActionResult update(int userId, [FromBody] UpdateUserOptionsRequest request)
        {
            _service.updateOptions(userId, request);
            return Ok();
        }
    }
}
