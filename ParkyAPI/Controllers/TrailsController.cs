using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{

    [Route("api/Trails")]
    //[Route("api/v{version:apiVersion}/trails")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(IMapper mapper, ITrailRepository trailRepo)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of trails.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var objList = _trailRepo.GetTrails();

            var objDto = new List<TrailDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual trail
        /// </summary>
        /// <param name="trailId">The Id of the Trail</param>
        /// <returns></returns>
        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrail(int trailId)
        {
            Trail objFromDb = _trailRepo.GetTrail(trailId);
            if(objFromDb!=null)
            {
                TrailDto trailDto = _mapper.Map<TrailDto>(objFromDb);
                return Ok(trailDto);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get trails innational park
        /// </summary>
        /// <param name="nationalParkId">The Id of the National Park</param>
        /// <returns></returns>
        [HttpGet("[action]/{trailId:int}", Name = "GetTrailsInNationalPark")]
        [ProducesResponseType(200, Type = typeof(List<TrailDto>))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {
            var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);
            if (objList == null)
            {
                return NotFound();
            }
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Create Trail
        /// </summary>
        /// <param name="trailDto">The Trail DTO</param>
        /// <returns></returns>
        [HttpPost( Name = "CreateTrail")]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }
            if(_trailRepo.TrailExists(trailDto.Name))
            {
                ModelState.AddModelError("","Trail exists!");
                return StatusCode(404, ModelState);
            }
            var trailObj = _mapper.Map<Trail>(trailDto);
            trailObj.DateCreated = DateTime.Now;
            if(!_trailRepo.CreateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when creating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetTrail", new { trailId = trailObj.Id }, trailObj);
        }


        /// <summary>
        ///  Update Trail
        /// </summary>
        /// <param name="trailId">The Id of the Trail</param>
        /// <param name="trailDto">The Trail DTO</param>
        /// <returns></returns>
        [HttpPatch("trailId:int", Name = "UpdateTrail")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateTrail(int trailId, [FromBody] TrailUpdateDto trailDto) 
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }

            var trailObj = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        /// <summary>
        ///  Delete Trail
        /// </summary>
        /// <param name="trailId">The Id of the Trail</param>
        /// <returns></returns>
        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteTrail(int trailId)
        { 
            if(!_trailRepo.TrailExists(trailId))
                {
                    return NotFound();
                }

            var trailObj = _trailRepo.GetTrail(trailId);

            if(!_trailRepo.DeleteTrail(trailObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {trailObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
