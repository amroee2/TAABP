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
        public async Task<IActionResult> AddNewImageAsync(int id, HotelImageDto image)
        {
            try
            {
                await _hotelService.AddNewImageAsync(id, image);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            return Ok();
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetHotelImage(int imageId)
        {
            try
            {
                var hotelImage = await _hotelService.GetHotelImageAsync(imageId);
                return Ok(hotelImage);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHotelImages(int id)
        {
            var hotelImages = await _hotelService.GetHotelImagesAsync(id);
            return Ok(hotelImages);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteHotelImage( int imageId)
        {
            try
            {
                await _hotelService.DeleteHotelImageAsync(imageId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdateHotelImage(int imageId, HotelImageDto imageUrl)
        {
            try
            {
                await _hotelService.UpdateHotelImageAsync(imageId, imageUrl);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
