using inzBackend.Entities;
using inzBackend.Models.ModuleModels;
using inzBackend.Models;
using inzBackend.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inzBackend.Helpers;
using inzBackend.Services.SectionActivityServices;

namespace inzBackend.Controllers
{
    [Route("api/activity")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class SectionActivityController : ControllerBase
    {
        private readonly ISectionActivityService _sectionActivityService;

        public SectionActivityController(ISectionActivityService sectionActivityService)
        {
            _sectionActivityService = sectionActivityService;
        }

        [HttpPost("log")]
        public ActionResult logActivity([FromBody] LogActivityRequest request)
        {
            _sectionActivityService.logActivity(request);
            return Ok();
        }
    }
}
