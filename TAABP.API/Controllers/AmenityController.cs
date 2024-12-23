using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.API.Controllers
{
    [Route("api/Hotel/{hotelId}/[controller]")]
    [ApiController]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityService _amenityService;
        public AmenityController(IAmenityService amenityService)
        {
            _amenityService = amenityService;
        }

        [HttpGet]
        public async Task<ActionResult<List<AmenityDto>>> GetHotelAmenitiesAsync(int hotelId)
        {
            try
            {
                return await _amenityService.GetHotelAmenitiesAsync(hotelId);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{amenityId}")]
        public async Task<ActionResult<AmenityDto>> GetAmenityAsync(int amenityId)
        {
            try
            {
                return await _amenityService.GetAmenityAsync(amenityId);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAmenityAsync(AmenityDto amenity)
        {
            try
            {
                await _amenityService.CreateAmenityAsync(amenity);
                return Created();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{amenityId}")]
        public async Task<ActionResult> UpdateAmenityAsync(int amenityId, AmenityDto amenity)
        {
            try
            {
                amenity.AmenityId = amenityId;
                await _amenityService.UpdateAmenityAsync(amenity);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{amenityId}")]
        public async Task<ActionResult> DeleteAmenityAsync(int amenityId, AmenityDto amenity)
        {
            try
            {
                amenity.AmenityId = amenityId;
                await _amenityService.DeleteAmenityAsync(amenity);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
