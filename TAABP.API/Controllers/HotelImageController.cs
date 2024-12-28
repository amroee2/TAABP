using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Hotels/{hotelId}/[controller]")]
    [ApiController]
    public class HotelImageController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public HotelImageController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewHotelImageAsync(int hotelId, HotelImageDto image)
        {
            try
            {
                var imageId = await _hotelService.CreateNewHotelImageAsync(hotelId, image);
                var hotelImage = await _hotelService.GetHotelImageByIdAsync(hotelId, imageId);
                return StatusCode(201, hotelImage);
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

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetHotelImageAsync(int hotelId, int imageId)
        {
            try
            {
                var hotelImage = await _hotelService.GetHotelImageByIdAsync(hotelId, imageId);
                return Ok(hotelImage);
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

        [HttpGet]
        public async Task<IActionResult> GetHotelImagesAsync(int hotelId)
        {
            var hotelImages = await _hotelService.GetHotelImagesAsync(hotelId);
            return Ok(hotelImages);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteHotelImageAsync(int hotelId, int imageId)
        {
            try
            {
                await _hotelService.GetHotelImageByIdAsync(hotelId, imageId);
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

        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdateHotelImageAsync(int hotelId, int imageId, HotelImageDto imageUrl)
        {
            imageUrl.HotelImageId = imageId;
            try
            {
                await _hotelService.UpdateHotelImageAsync(hotelId, imageId, imageUrl);
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
