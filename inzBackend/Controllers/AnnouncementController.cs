using inzBackend.Models.AnnouncementModels;
using inzBackend.Services.AnnouncementsServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/announcement")]
    [ApiController]
    [Authorize]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announcementService;
        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<AnnouncementDto>> GetAll()
        {
            return _announcementService.GetAll();
        }

        [HttpGet("inbox")]
        public ActionResult<List<AnnouncementInboxDto>> GetInbox()
        {
            return _announcementService.GetInbox();
        }

        [HttpGet("unread-count")]
        public ActionResult<UnreadCountDto> GetUnreadCount()
        {
            return _announcementService.GetUnreadCount();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([FromBody] CreateAnnouncementRequest request)
        {
            _announcementService.Create(request);
            return Ok();
        }

        [HttpPatch("{recipientId}/read")]
        public ActionResult MarkRead([FromRoute] int recipientId)
        {
            _announcementService.MarkRead(recipientId);
            return Ok();
        }

        [HttpPatch("{recipientId}/signup")]
        public ActionResult SignUp([FromRoute] int recipientId)
        {
            _announcementService.SignUp(recipientId);
            return Ok();
        }

        [HttpPatch("{recipientId}/vote")]
        public ActionResult Vote([FromRoute] int recipientId, [FromQuery] bool value)
        {
            _announcementService.Vote(recipientId, value);
            return Ok();
        }

        [HttpPatch("read-all")]
        public ActionResult MarkAllRead()
        {
            _announcementService.MarkAllRead();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] int id)
        {
            _announcementService.Delete(id);
            return NoContent();
        }

        [HttpGet("{id}/details")]
        [Authorize(Roles = "Admin")]
        public ActionResult<AnnouncementDetailsDto> GetDetails([FromRoute] int id)
        {
            return _announcementService.GetDetails(id);
        }
    }
}