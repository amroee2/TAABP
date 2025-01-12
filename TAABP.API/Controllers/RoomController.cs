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
    [Route("api/Hotels/{hotelId}/Rooms")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger _logger;
        private readonly IValidator<RoomDto> _roomValidator;
        public RoomController(IRoomService roomService, IValidator<RoomDto> validator)
        {
            _roomService = roomService;
            _logger = Log.ForContext<RoomController>();
            _roomValidator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsAsync(int hotelId)
        {
            _logger.Information("Fetching all rooms for hotel with ID {HotelId}", hotelId);
            try
            {
                var rooms = await _roomService.GetRoomsAsync(hotelId);
                _logger.Information("Successfully fetched all rooms for hotel with ID {HotelId}", hotelId);
                return Ok(rooms);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("No rooms found for hotel with ID {HotelId}", hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching all rooms for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetRoomAsync(int hotelId, int roomId)
        {
            _logger.Information("Fetching room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
            try
            {
                var room = await _roomService.GetRoomByIdAsync(hotelId, roomId);
                _logger.Information("Successfully fetched room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
                return Ok(room);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room with ID {RoomId} not found for hotel with ID {HotelId}", roomId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoomAsync(int hotelId, RoomDto roomDto)
        {
            _logger.Information("Creating a new room for hotel with ID {HotelId}", hotelId);
            try
            {
                await _roomValidator.ValidateAndThrowAsync(roomDto);
                roomDto.HotelId = hotelId;
                var id = await _roomService.CreateRoomAsync(roomDto);
                var room = await _roomService.GetRoomByIdAsync(hotelId, id);
                _logger.Information("Successfully created a new room with ID {RoomId} for hotel with ID {HotelId}", id, hotelId);
                return StatusCode(201, room);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating a new room for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoomAsync(int hotelId, int roomId, RoomDto roomDto)
        {
            _logger.Information("Updating room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
            try
            {
                await _roomValidator.ValidateAndThrowAsync(roomDto);
                roomDto.HotelId = hotelId;
                roomDto.RoomId = roomId;
                await _roomService.UpdateRoomAsync(roomDto);
                _logger.Information("Successfully updated room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room with ID {RoomId} not found for hotel with ID {HotelId}", roomId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{roomId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRoomAsync(int hotelId, int roomId)
        {
            _logger.Information("Deleting room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
            try
            {
                await _roomService.DeleteRoomAsync(hotelId, roomId);
                _logger.Information("Successfully deleted room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room with ID {RoomId} not found for hotel with ID {HotelId}", roomId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting room with ID {RoomId} for hotel with ID {HotelId}", roomId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
