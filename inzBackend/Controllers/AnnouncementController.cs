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
        public ActionResult markRead(int recipientId)
        {
            _announcementService.markRead(recipientId);
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
        public ActionResult delete(int id)
        {
            _announcementService.delete(id);
            return NoContent();
        }
    }
}
