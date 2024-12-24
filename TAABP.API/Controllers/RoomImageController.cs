using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Hotel/{hotelId}/Room/{roomId}/[controller]")]
    [ApiController]
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
            roomImageDto.RoomId = roomId;
            await _roomImageService.CreateRoomImageAsync(roomImageDto);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomImageAsync(int id)
        {
            try
            {
                var roomImage = await _roomImageService.GetRoomImageAsync(id);
                return Ok(roomImage);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomImagesAsync(int roomId)
        {
            var roomImages = await _roomImageService.GetRoomImagesAsync(roomId);
            return Ok(roomImages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomImageAsync(int id)
        {
            try
            {
                await _roomImageService.DeleteRoomImageAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
