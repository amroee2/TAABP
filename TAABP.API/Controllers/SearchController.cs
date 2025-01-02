using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public SearchController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetFilteredHotelsAsync([FromBody] FilterOptionsDto hotelFilter)
        {
            var hotels = await _hotelService.GetFilteredHotelsAsync(hotelFilter);
            return Ok(hotels);
        }
    }
}
