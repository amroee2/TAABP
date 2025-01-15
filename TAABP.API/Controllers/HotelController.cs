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
    [Route("api/Cities/{cityId}/Hotels")]
    [ApiController]
    [Authorize]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IReviewService _reviewService;
        private readonly ILogger _logger;
        private readonly IValidator<HotelDto> _hotelValidator;
        public HotelController(IHotelService hotelService,
            IReviewService reviewService, IValidator<HotelDto> validator)
        {
            _hotelService = hotelService;
            _reviewService = reviewService;
            _logger = Log.ForContext<HotelController>();
            _hotelValidator = validator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateHotelAsync(int cityId, HotelDto hotelDto)
        {
            _logger.Information("Creating a new hotel in city with ID {CityId}", cityId);
            try
            {
                await _hotelValidator.ValidateAndThrowAsync(hotelDto);
                var hotelId = await _hotelService.CreateHotelAsync(cityId, hotelDto);
                var hotel = await _hotelService.GetHotelByIdAsync(cityId, hotelId);
                _logger.Information("Successfully created hotel with ID {HotelId}", hotelId);
                return StatusCode(201, hotel);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("City with ID {CityId} not found", cityId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating a new hotel");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelByIdAsync(int cityId, int id)
        {
            _logger.Information("Fetching hotel with ID {HotelId} in city with ID {CityId}", id, cityId);
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(cityId, id);
                _logger.Information("Successfully fetched hotel with ID {HotelId}", id);
                return Ok(hotel);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching hotel with ID {HotelId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHotelsAsync(int cityId)
        {
            _logger.Information("Fetching hotels in city with ID {CityId}", cityId);
            var hotels = await _hotelService.GetHotelsAsync(cityId);
            _logger.Information("Successfully fetched {HotelCount} hotels in city with ID {CityId}", hotels.Count, cityId);
            return Ok(hotels);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotelAsync(int cityId, int id)
        {
            _logger.Information("Deleting hotel with ID {HotelId} in city with ID {CityId}", id, cityId);
            try
            {
                await _hotelService.DeleteHotelAsync(cityId, id);
                _logger.Information("Successfully deleted hotel with ID {HotelId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting hotel with ID {HotelId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHotelAsync(int cityId, int id, HotelDto hotelDto)
        {
            _logger.Information("Updating hotel with ID {HotelId} in city with ID {CityId}", id, cityId);
            try
            {
                await _hotelValidator.ValidateAndThrowAsync(hotelDto);
                hotelDto.HotelId = id;
                await _hotelService.UpdateHotelAsync(cityId, hotelDto);
                _logger.Information("Successfully updated hotel with ID {HotelId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating hotel with ID {HotelId}", id);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{hotelId}/Reviews")]
        public async Task<IActionResult> GetAllUserReviewsAsync(int hotelId)
        {
            _logger.Information("Fetching all reviews for hotel with ID {HotelId}", hotelId);
            try
            {
                var reviews = await _reviewService.GetAllHotelReviewsAsync(hotelId);
                _logger.Information("Successfully fetched {ReviewCount} reviews for hotel with ID {HotelId}", reviews.Count, hotelId);
                return Ok(reviews);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", hotelId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching reviews for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
