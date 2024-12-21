using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Hotels/{id}/[controller]")]
    [ApiController]
    public class HotelImageController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public HotelImageController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewImageAsync(int id, HotelImageDto hotelImageDto)
        {
            try
            {
                await _hotelService.AddNewImageAsync(id, hotelImageDto);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return Ok();
        }
    }
}
