using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;

namespace TAABP.API.Controllers
{
    [Route("api/Cities/{cityId}/Hotels")]
    [ApiController]
    [Authorize]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IReviewService _reviewService;
        public HotelController(IHotelService hotelService, IReviewService reviewService)
        {
            _hotelService = hotelService;
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotelAsync(int cityId, HotelDto hotelDto)
        {
            try
            {
                var hotelId = await _hotelService.CreateHotelAsync(cityId, hotelDto);
                var hotel = await _hotelService.GetHotelByIdAsync(hotelId);
                return StatusCode(201, hotel);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelByIdAsync(int id)
        {
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                return Ok(hotel);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHotelsAsync()
        {
            var hotels = await _hotelService.GetHotelsAsync();
            return Ok(hotels);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelAsync(int id)
        {
            try
            {
                await _hotelService.DeleteHotelAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelAsync(int cityId, int id, HotelDto hotelDto)
        {
            hotelDto.HotelId = id;
            try
            {
                await _hotelService.UpdateHotelAsync(cityId, hotelDto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchHotelAsync(int id, JsonPatchDocument<HotelDto> patchDoc)
        {
            try
            {
                var hotel = await _hotelService.GetHotelByIdAsync(id);
                patchDoc.ApplyTo(hotel, (error) => ModelState.AddModelError(error.AffectedObject.ToString(), error.ErrorMessage));
                if (!TryValidateModel(hotel))
                {
                    return ValidationProblem(ModelState);
                }
                await _hotelService.UpdateHotelAsync(id, hotel);
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

        [HttpGet("{hotelId}/Reviews")]
        public async Task<IActionResult> GetAllUserReviewsAsync(int hotelId)
        {
            try
            {
                var reviews = await _reviewService.GetAllHotelReviewsAsync(hotelId);
                return Ok(reviews);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
