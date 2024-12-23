using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Hotel/{hotelId}/[controller]")]
    [ApiController]
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
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomAsync(int roomId)
        {
            try
            {
                var room = await _roomService.GetRoomAsync(roomId);
                return Ok(room);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync(int hotelId, RoomDto roomDto)
        {
            try
            {
                roomDto.HotelId = hotelId;
                await _roomService.CreateRoomAsync(roomDto);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        }

        [HttpDelete("{roomId}")]
        public async Task<IActionResult> DeleteRoomAsync(int roomId)
        {
            try
            {
                await _roomService.DeleteRoomAsync(roomId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{roomId}")]
        public async Task<IActionResult> PatchRoomAsync(int hotelId, int roomId, JsonPatchDocument<RoomDto> patchDoc)
        {
            try
            {
                var room = await _roomService.GetRoomAsync(roomId);
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
        }
    }
}
