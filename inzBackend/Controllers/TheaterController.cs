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
        public ActionResult<List<TheaterItemDto>> GetAll()
        {
            return _theaterService.getAll();
        }
            
        [HttpGet("repertoire")]
        public ActionResult<List<RepertoireItemDto>> GetRepertoire()
        {
            return _theaterService.getRepertoire();
        } 

        [HttpGet("{id}")]
        public ActionResult<TheaterItemDto> GetById([FromRoute] int id)
        {
            return _theaterService.getById(id);
        }
            
        [HttpPost]
        public ActionResult<TheaterItemDto> Create([FromBody] CreateTheaterItemRequest request)
        {
            return _theaterService.create(request);
        }
            
        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateTheaterItemRequest request)
        {
            _theaterService.update(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _theaterService.delete(id);
            return NoContent();
        }

        [HttpPatch("{id}/toggle")]
        public ActionResult Toggle([FromRoute] int id)
        {
            _theaterService.toggleActive(id);
            return Ok();
        }
    }
}
