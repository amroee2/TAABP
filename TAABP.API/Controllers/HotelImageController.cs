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
    [Route("api/Hotels/{hotelId}/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelImageController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly ILogger _logger;
        private readonly IValidator<HotelImageDto> _hotelImageValidator;
        public HotelImageController(IHotelService hotelService, IValidator<HotelImageDto> validator)
        {
            _hotelService = hotelService;
            _logger = Log.ForContext<HotelImageController>();
            _hotelImageValidator = validator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewHotelImageAsync(int hotelId, HotelImageDto image)
        {
            _logger.Information("Creating a new image for hotel with ID {HotelId}", hotelId);
            try
            {
                await _hotelImageValidator.ValidateAndThrowAsync(image);
                var imageId = await _hotelService.CreateNewHotelImageAsync(hotelId, image);
                var hotelImage = await _hotelService.GetHotelImageByIdAsync(hotelId, imageId);
                _logger.Information("Successfully created image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return StatusCode(201, hotelImage);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating a new image for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetHotelImageAsync(int hotelId, int imageId)
        {
            _logger.Information("Fetching image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
            try
            {
                var hotelImage = await _hotelService.GetHotelImageByIdAsync(hotelId, imageId);
                _logger.Information("Successfully fetched image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return Ok(hotelImage);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Image with ID {ImageId} for hotel with ID {HotelId} not found", imageId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetHotelImagesAsync(int hotelId)
        {
            _logger.Information("Fetching images for hotel with ID {HotelId}", hotelId);
            try
            {
                var hotelImages = await _hotelService.GetHotelImagesAsync(hotelId);
                _logger.Information("Successfully fetched {HotelImageCount} images for hotel with ID {HotelId}", hotelImages.Count, hotelId);
                return Ok(hotelImages);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching images for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteHotelImageAsync(int hotelId, int imageId)
        {
            _logger.Information("Deleting image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
            try
            {
                await _hotelService.DeleteHotelImageAsync(hotelId, imageId);
                _logger.Information("Successfully deleted image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Image with ID {ImageId} for hotel with ID {HotelId} not found", imageId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{imageId}")]
        public async Task<IActionResult> UpdateHotelImageAsync(int hotelId, int imageId, HotelImageDto imageUrl)
        {
            _logger.Information("Updating image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
            try
            {
                await _hotelImageValidator.ValidateAndThrowAsync(imageUrl);
                imageUrl.HotelImageId = imageId;
                await _hotelService.UpdateHotelImageAsync(hotelId, imageId, imageUrl);
                _logger.Information("Successfully updated image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Image with ID {ImageId} for hotel with ID {HotelId} not found", imageId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating image with ID {ImageId} for hotel with ID {HotelId}", imageId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
