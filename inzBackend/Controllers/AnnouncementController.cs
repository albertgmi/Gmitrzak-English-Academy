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
        public List<AnnouncementDto> getAll()
        {
            return _announcementService.getAll();
        }

        [HttpGet("inbox")]
        public List<AnnouncementInboxDto> getInbox()
        {
            return _announcementService.getInbox();
        }

        [HttpGet("unread-count")]
        public UnreadCountDto getUnreadCount()
        {
            return _announcementService.getUnreadCount();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult create([FromBody] CreateAnnouncementRequest request)
        {
            _announcementService.create(request);
            return Ok();
        }

        [HttpPatch("{recipientId}/read")]
        public ActionResult markRead([FromRoute] int recipientId)
        {
            _announcementService.markRead(recipientId);
            return Ok();
        }

        [HttpPatch("{recipientId}/signup")]
        public ActionResult signUp([FromRoute] int recipientId)
        {
            _announcementService.signUp(recipientId);
            return Ok();
        }

        [HttpPatch("{recipientId}/vote")]
        public ActionResult vote([FromRoute] int recipientId, [FromQuery] bool value)
        {
            _announcementService.vote(recipientId, value);
            return Ok();
        }

        [HttpPatch("read-all")]
        public ActionResult markAllRead()
        {
            _announcementService.markAllRead();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult delete([FromRoute] int id)
        {
            _announcementService.delete(id);
            return NoContent();
        }

        [HttpGet("{id}/details")]
        [Authorize(Roles = "Admin")]
        public AnnouncementDetailsDto getDetails([FromRoute] int id)
        {
            return _announcementService.getDetails(id);
        }
    }
}