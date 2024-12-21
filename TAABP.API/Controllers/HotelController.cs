using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Application.Services;
using TAABP.Core;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotelAsync(HotelDto hotelDto)
        {
            await _hotelService.CreateHotelAsync(hotelDto);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelAsync(int id)
        {
            try
            {
                var hotel = await _hotelService.GetHotelAsync(id);
                return Ok(hotel);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelAsync(int id, HotelDto hotelDto)
        {
            try
            {
                await _hotelService.UpdateHotelAsync(id, hotelDto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchHotelAsync(int id, JsonPatchDocument<HotelDto> patchDoc)
        {
            try
            {
                var hotel = await _hotelService.GetHotelAsync(id);
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
    }
}
