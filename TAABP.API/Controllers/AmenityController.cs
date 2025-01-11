using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.API.Controllers
{
    [Route("api/Hotel/{hotelId}/[controller]")]
    [ApiController]
    [Authorize]
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{amenityId}")]
        public async Task<ActionResult<AmenityDto>> GetAmenityByIdAsync(int hotelId, int amenityId)
        {
            try
            {
                return await _amenityService.GetAmenityByIdAsync(hotelId, amenityId);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAmenityAsync(int hotelId, AmenityDto amenity)
        {
            try
            {
                amenity.HotelId = hotelId;
                var amenityId = await _amenityService.CreateAmenityAsync(amenity);
                var amenityDto = await _amenityService.GetAmenityByIdAsync(amenity.HotelId, amenityId);
                return StatusCode(201, amenityDto);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{amenityId}")]
        public async Task<ActionResult> UpdateAmenityAsync(int hotelId, int amenityId, AmenityDto amenity)
        {
            try
            {
                amenity.AmenityId = amenityId;
                amenity.HotelId = hotelId;
                await _amenityService.UpdateAmenityAsync( amenity);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{amenityId}")]
        public async Task<ActionResult> DeleteAmenityAsync(int hotelId, int amenityId)
        {
            try
            {
                await _amenityService.DeleteAmenityAsync(hotelId, amenityId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
