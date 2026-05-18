using inzBackend.Models.StreamEntryModels;
using inzBackend.Services.StreamEntryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/stream")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StreamController : ControllerBase
    {
        private readonly IStreamService _streamService;

        public StreamController(IStreamService streamService)
        {
            _streamService = streamService;
        }

        [HttpGet]
        public List<StreamEntryListDto> getAll([FromQuery] int? userId) =>
            _streamService.getAll(userId);

        [HttpPost]
        public ActionResult create([FromBody] CreateStreamEntryRequest request)
        {
            _streamService.create(request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult delete(int id)
        {
            _streamService.delete(id);
            return NoContent();
        }

        [HttpDelete]
        public ActionResult deleteMultiple([FromBody] DeleteMultipleStreamRequest request)
        {
            _streamService.deleteMultiple(request.Ids);
            return NoContent();
        }
    }
}
