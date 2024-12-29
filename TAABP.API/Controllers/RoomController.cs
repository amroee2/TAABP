using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Hotel/{hotelId}/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsAsync(int hotelId)
        {
            try
            {
                var rooms = await _roomService.GetRoomsAsync(hotelId);
                return Ok(rooms);
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

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomAsync(int hotelId, int roomId)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(hotelId, roomId);
                return Ok(room);
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

        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync(int hotelId, RoomDto roomDto)
        {
            try
            {
                roomDto.HotelId = hotelId;
                var id = await _roomService.CreateRoomAsync(roomDto);
                var room = await _roomService.GetRoomByIdAsync(hotelId, id);
                return StatusCode(201, room);
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

        [HttpPut("{roomId}")]
        public async Task<IActionResult> UpdateRoomAsync(int hotelId, int roomId, RoomDto roomDto)
        {
            try
            {
                roomDto.HotelId = hotelId;
                roomDto.RoomId = roomId;
                await _roomService.UpdateRoomAsync(roomDto);
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

        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoomAsync(int hotelId, int roomId)
        {
            try
            {
                await _roomService.DeleteRoomAsync(hotelId, roomId);
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

        [HttpPatch("{roomId}")]
        public async Task<IActionResult> PatchRoomAsync(int hotelId, int roomId, JsonPatchDocument<RoomDto> patchDoc)
        {
            try
            {
                var room = await _roomService.GetRoomByIdAsync(hotelId, roomId);
                patchDoc.ApplyTo(room);
                room.HotelId = hotelId;
                room.RoomId = roomId;
                await _roomService.UpdateRoomAsync(room);
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
