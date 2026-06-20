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
            return _theaterService.GetAll();
        }
            
        [HttpGet("repertoire")]
        public ActionResult<List<RepertoireItemDto>> GetRepertoire()
        {
            return _theaterService.GetRepertoire();
        } 

        [HttpGet("{id}")]
        public ActionResult<TheaterItemDto> GetById([FromRoute] int id)
        {
            return _theaterService.GetById(id);
        }
            
        [HttpPost]
        public ActionResult<TheaterItemDto> Create([FromBody] CreateTheaterItemRequest request)
        {
            return _theaterService.Create(request);
        }
            
        [HttpPut("{id}")]
        public ActionResult Update([FromRoute] int id, [FromBody] UpdateTheaterItemRequest request)
        {
            _theaterService.Update(id, request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            _theaterService.Delete(id);
            return NoContent();
        }

        [HttpPatch("{id}/toggle")]
        public ActionResult Toggle([FromRoute] int id)
        {
            _theaterService.ToggleActive(id);
            return Ok();
        }
    }
}
