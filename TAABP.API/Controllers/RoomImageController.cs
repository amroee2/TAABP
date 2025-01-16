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
    [Route("api/Rooms/{roomId}/RoomImages")]
    [ApiController]
    [Authorize]
    public class RoomImageController : ControllerBase
    {
        private readonly IRoomService _roomImageService;
        private readonly ILogger _logger;
        public RoomImageController(IRoomService roomImageService)
        {
            _roomImageService = roomImageService;
            _logger = Log.ForContext<RoomImageController>();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoomImageAsync(int roomId, RoomImageDto roomImageDto)
        {
            _logger.Information("Adding room image for room with ID {RoomId}", roomId);
            try
            {
                roomImageDto.RoomId = roomId;
                var roomImageId = await _roomImageService.CreateRoomImageAsync(roomImageDto);
                var roomImage = await _roomImageService.GetRoomImageByIdAsync(roomId, roomImageId);
                return StatusCode(201, roomImage);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room with ID {RoomId} not found", roomId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding room image for room with ID {RoomId}", roomId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomImageAsync(int roomId, int id)
        {
            _logger.Information("Fetching room image with ID {RoomImageId} for room with ID {RoomId}", id, roomId);
            try
            {
                var roomImage = await _roomImageService.GetRoomImageByIdAsync(roomId, id);
                _logger.Information("Successfully fetched room image with ID {RoomImageId} for room with ID {RoomId}", id, roomId);
                return Ok(roomImage);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room image with ID {RoomImageId} not found for room with ID {RoomId}", id, roomId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching room image with ID {RoomImageId} for room with ID {RoomId}", id, roomId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomImagesAsync(int roomId)
        {
            _logger.Information("Fetching all room images for room with ID {RoomId}", roomId);
            try
            {
                var roomImages = await _roomImageService.GetRoomImagesAsync(roomId);
                _logger.Information("Successfully fetched all room images for room with ID {RoomId}", roomId);
                return Ok(roomImages);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("No room images found for room with ID {RoomId}", roomId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching all room images for room with ID {RoomId}", roomId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomImageAsync(int roomId, int id)
        {
            _logger.Information("Deleting room image with ID {RoomImageId} for room with ID {RoomId}", id, roomId);
            try
            {
                await _roomImageService.DeleteRoomImageAsync(roomId, id);
                _logger.Information("Successfully deleted room image with ID {RoomImageId} for room with ID {RoomId}", id, roomId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room image with ID {RoomImageId} for room with ID {RoomId} not found", id, roomId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting room image with ID {RoomImageId} for room with ID {RoomId}", id, roomId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
