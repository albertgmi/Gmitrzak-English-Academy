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
            return _announcementService.getAll();
        }

        [HttpGet("inbox")]
        public ActionResult<List<AnnouncementInboxDto>> GetInbox()
        {
            return _announcementService.getInbox();
        }

        [HttpGet("unread-count")]
        public ActionResult<UnreadCountDto> GetUnreadCount()
        {
            return _announcementService.getUnreadCount();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([FromBody] CreateAnnouncementRequest request)
        {
            _announcementService.create(request);
            return Ok();
        }

        [HttpPatch("{recipientId}/read")]
        public ActionResult MarkRead([FromRoute] int recipientId)
        {
            _announcementService.markRead(recipientId);
            return Ok();
        }

        [HttpPatch("{recipientId}/signup")]
        public ActionResult SignUp([FromRoute] int recipientId)
        {
            _announcementService.signUp(recipientId);
            return Ok();
        }

        [HttpPatch("{recipientId}/vote")]
        public ActionResult Vote([FromRoute] int recipientId, [FromQuery] bool value)
        {
            _announcementService.vote(recipientId, value);
            return Ok();
        }

        [HttpPatch("read-all")]
        public ActionResult MarkAllRead()
        {
            _announcementService.markAllRead();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete([FromRoute] int id)
        {
            _announcementService.delete(id);
            return NoContent();
        }

        [HttpGet("{id}/details")]
        [Authorize(Roles = "Admin")]
        public ActionResult<AnnouncementDetailsDto> GetDetails([FromRoute] int id)
        {
            return _announcementService.getDetails(id);
        }
    }
}