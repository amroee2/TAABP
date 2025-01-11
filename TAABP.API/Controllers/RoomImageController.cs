using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Room/{roomId}/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomImageController : ControllerBase
    {
        private readonly IRoomService _roomImageService;

        public RoomImageController(IRoomService roomImageService)
        {
            _roomImageService = roomImageService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoomImageAsync(int roomId, RoomImageDto roomImageDto)
        {
            try
            {
                roomImageDto.RoomId = roomId;
                var roomImageId = await _roomImageService.CreateRoomImageAsync(roomImageDto);
                var roomImage = await _roomImageService.GetRoomImageByIdAsync(roomId, roomImageId);
                return StatusCode(201, roomImage);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomImageAsync(int roomId, int id)
        {
            try
            {
                var roomImage = await _roomImageService.GetRoomImageByIdAsync(roomId, id);
                return Ok(roomImage);
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
        public async Task<IActionResult> GetRoomImagesAsync(int roomId)
        {
            try
            {
                var roomImages = await _roomImageService.GetRoomImagesAsync(roomId);
                return Ok(roomImages);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomImageAsync(int roomId, int id)
        {
            try
            {
                await _roomImageService.DeleteRoomImageAsync(roomId, id);
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
