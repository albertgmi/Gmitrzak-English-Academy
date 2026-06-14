using inzBackend.Models.TheaterItemsModels;
using inzBackend.Services.TheaterItemServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inzBackend.Controllers
{
    [Route("api/theater")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        [HttpGet]
        public ActionResult<List<TheaterItemDto>> getAll()
        {
            return _theaterService.getAll();
        }
            
        [HttpGet("repertoire")]
        public ActionResult<List<RepertoireItemDto>> getRepertoire()
        {
            return _theaterService.getRepertoire();
        } 

        [HttpGet("{id}")]
        public ActionResult<TheaterItemDto> getById([FromRoute] int id)
        {
            return _theaterService.getById(id);
        }
            
        [HttpPost]
        public ActionResult<TheaterItemDto> create([FromBody] CreateTheaterItemRequest request)
        {
            return _theaterService.create(request);
        }
            
        [HttpPut("{id}")]
        public ActionResult update([FromRoute] int id, [FromBody] UpdateTheaterItemRequest request)
        {
            _theaterService.update(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult delete([FromRoute] int id)
        {
            _theaterService.delete(id);
            return NoContent();
        }

        [HttpPatch("{id}/toggle")]
        public ActionResult toggle([FromRoute] int id)
        {
            _theaterService.toggleActive(id);
            return Ok();
        }
    }
}
