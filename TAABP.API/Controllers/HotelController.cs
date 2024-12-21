using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

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
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
