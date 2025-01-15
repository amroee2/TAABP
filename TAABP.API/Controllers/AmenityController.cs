using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using ILogger = Serilog.ILogger;

namespace TAABP.API.Controllers
{
    [Route("api/Hotels/{hotelId}/Amenities")]
    [ApiController]
    [Authorize]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityService _amenityService;
        private readonly ILogger _logger;
        private readonly IValidator<AmenityDto> _amenityValidator;


        public AmenityController(IAmenityService amenityService, IValidator<AmenityDto> amenityValidator)
        {
            _amenityService = amenityService;
            _logger = Log.ForContext<AmenityController>();
            _amenityValidator = amenityValidator;
        }

        [HttpGet]
        public async Task<ActionResult<List<AmenityDto>>> GetHotelAmenitiesAsync(int hotelId)
        {
            _logger.Information("Fetching amenities for hotel with ID {HotelId}", hotelId);
            try
            {
                var amenities = await _amenityService.GetHotelAmenitiesAsync(hotelId);
                _logger.Information("Successfully fetched {AmenityCount} amenities for hotel {HotelId}", amenities.Count, hotelId);
                return amenities;
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching amenities for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{amenityId}")]
        public async Task<ActionResult<AmenityDto>> GetAmenityByIdAsync(int hotelId, int amenityId)
        {
            _logger.Information("Fetching amenity with ID {AmenityId} for hotel with ID {HotelId}", amenityId, hotelId);
            try
            {
                return await _amenityService.GetAmenityByIdAsync(hotelId, amenityId);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Amenity with ID {AmenityId} not found for hotel with ID {HotelId}", amenityId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching amenity with ID {AmenityId} for hotel with ID {HotelId}", amenityId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateAmenityAsync(int hotelId, AmenityDto amenity)
        {
            _logger.Information("Creating a new amenity for hotel {HotelId}", hotelId);
            try
            {
                await _amenityValidator.ValidateAndThrowAsync(amenity);
                amenity.HotelId = hotelId;
                var amenityId = await _amenityService.CreateAmenityAsync(amenity);
                var amenityDto = await _amenityService.GetAmenityByIdAsync(amenity.HotelId, amenityId);
                return StatusCode(201, amenityDto);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Failed to create amenity for hotel {HotelId}: {ErrorMessage}", hotelId, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating an amenity for hotel {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{amenityId}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> UpdateAmenityAsync(int hotelId, int amenityId, AmenityDto amenity)
        {
            _logger.Information("Updating amenity {AmenityId} for hotel {HotelId}", amenityId, hotelId);
            try
            {
                await _amenityValidator.ValidateAndThrowAsync(amenity);
                amenity.AmenityId = amenityId;
                amenity.HotelId = hotelId;
                await _amenityService.UpdateAmenityAsync(amenity);
                _logger.Information("Successfully updated amenity {AmenityId} for hotel {HotelId}", amenityId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Failed to update amenity {AmenityId} for hotel {HotelId}: {ErrorMessage}", amenityId, hotelId, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating amenity {AmenityId} for hotel {HotelId}", amenityId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{amenityId}")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> DeleteAmenityAsync(int hotelId, int amenityId)
        {
            _logger.Information("Deleting amenity {AmenityId} for hotel {HotelId}", amenityId, hotelId);
            try
            {
                await _amenityService.DeleteAmenityAsync(hotelId, amenityId);
                _logger.Information("Successfully deleted amenity {AmenityId} for hotel {HotelId}", amenityId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Failed to delete amenity {AmenityId} for hotel {HotelId}: {ErrorMessage}", amenityId, hotelId, ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting amenity {AmenityId} for hotel {HotelId}", amenityId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
