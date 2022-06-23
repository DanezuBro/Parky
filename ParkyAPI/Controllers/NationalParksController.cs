using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/NationalParks")]
    //[Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    //[EnableCors("AllowAll")]
    //[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNationalParks")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(IMapper mapper, INationalParkRepository npRepo)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get list of national parks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var objList = _npRepo.GetNationalParks();

            var objDto = new List<NationalParkDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }

            return Ok(objDto);
        }

        /// <summary>
        /// Get individual national park
        /// </summary>
        /// <param name="nationalParkId">The Id of the National Park</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            NationalPark objFromDb = _npRepo.GetNationalPark(nationalParkId);
            if (objFromDb != null)
            {
                NationalParkDto nationalParkDto = _mapper.Map<NationalParkDto>(objFromDb);
                return Ok(nationalParkDto);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        ///  Create National Park
        /// </summary>
        /// <param name="nationalParkDto">The National Park DTO</param>
        /// <returns></returns>
        [HttpPost(Name = "CreateNationalPark")]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }
            if (_npRepo.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park exists!");
                return StatusCode(404, ModelState);
            }
            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when creating the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id }, nationalParkObj);
        }

        /// <summary>
        ///  Update National Park
        /// </summary>
        /// <param name="nationalParkId">The Id of the National Park</param>
        /// <param name="nationalParkDto">Teh National Park DTO</param>
        /// <returns></returns>
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
            if (!_npRepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();

        }

        //[HttpPatch("nationalParkId:int", Name = "UpdateNationalPark")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] JsonPatchDocument nationalParkDto)
        //{
        //    if (nationalParkDto == null)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);
        //    nationalParkDto.ApplyTo(nationalParkObj);
        //    if (!_npRepo.UpdateNationalPark(nationalParkObj))
        //    {
        //        ModelState.AddModelError("", $"Something went wrong when updating the record {nationalParkObj.Name}");
        //        return StatusCode(500, ModelState);
        //    }
        //    return NoContent();
        //}

        /// <summary>
        ///  Delete National Park
        /// </summary>
        /// <param name="nationalParkId">The Id of the National Park</param>
        /// <returns></returns>
        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        { 
            if(!_npRepo.NationalParkExists(nationalParkId))
                {
                    return NotFound();
                }

            var nationalParkObj = _npRepo.GetNationalPark(nationalParkId);

            if(!_npRepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something went wrong when deleting the record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
