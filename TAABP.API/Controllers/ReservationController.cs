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
    [Route("api/Users/{userId}/Rooms/{roomId}/Reservations")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger _logger;
        private readonly IValidator<ReservationDto> _reservationValidator;
        private readonly IUserService _userService;
        public ReservationController(IReservationService reservationService,
            IValidator<ReservationDto> validator,
            IUserService userService)
        {
            _reservationService = reservationService;
            _logger = Log.ForContext<ReservationController>();
            _reservationValidator = validator;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservationDto>>> GetReservations(string userId)
        {
            _logger.Information("Fetching all reservations");
            try
            {
                if(userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to all reservations");
                    return Unauthorized();
                }
                var reservations = await _reservationService.GetReservationsAsync();
                _logger.Information("Successfully fetched all reservations");
                return Ok(reservations);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("No reservations found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching all reservations");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(string userId, int roomId, int id)
        {
            _logger.Information("Fetching reservation with ID {ReservationId}", id);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to reservation with ID {ReservationId}", id);
                    return Unauthorized();
                }
                var reservation = await _reservationService.GetReservationByIdAsync(userId, roomId, id);
                _logger.Information("Successfully fetched reservation with ID {ReservationId}", id);
                return Ok(reservation);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Reservation with ID {ReservationId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching reservation with ID {ReservationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateReservation(string userId, int roomId, ReservationDto reservationDto)
        {
            _logger.Information("Creating a new reservation for user with ID {UserId} and room with ID {RoomId}", userId, roomId);
            try
            {
                if(userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to create a new reservation for user with ID {UserId} and room with ID {RoomId}", userId, roomId);
                    return Unauthorized();
                }
                await _reservationValidator.ValidateAndThrowAsync(reservationDto);
                reservationDto.UserId = userId;
                reservationDto.RoomId = roomId;
                var reservationId = await _reservationService.CreateReservationAsync(reservationDto);
                var reservation = await _reservationService.GetReservationByIdAsync(userId, roomId, reservationId);
                _logger.Information("Successfully created a new reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId}", reservationId, userId, roomId);
                return StatusCode(201, reservation);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} or room with ID {RoomId} not found", userId, roomId);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warning("Failed to create a new reservation for user with ID {UserId} and room with ID {RoomId}", userId, roomId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while creating a new reservation for user with ID {UserId} and room with ID {RoomId}", userId, roomId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{reservationId}")]
        public async Task<ActionResult> UpdateReservation(int reservationId, string userId, int roomId, ReservationDto reservationDto)
        {
            _logger.Information("Updating reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId}", reservationId, userId, roomId);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to update reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId}", reservationId, userId, roomId);
                    return Unauthorized();
                }
                await _reservationValidator.ValidateAndThrowAsync(reservationDto);
                reservationDto.ReservationId = reservationId;
                reservationDto.UserId = userId;
                reservationDto.RoomId = roomId;
                await _reservationService.UpdateReservationAsync(reservationDto);
                _logger.Information("Successfully updated reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId}", reservationId, userId, roomId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId} not found", reservationId, userId, roomId);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warning("Failed to update reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId}", reservationId, userId, roomId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating reservation with ID {ReservationId} for user with ID {UserId} and room with ID {RoomId}", reservationId, userId, roomId);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReservation(string userId, int id)
        {
            _logger.Information("Deleting reservation with ID {ReservationId}", id);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to delete reservation with ID {ReservationId}", id);
                    return Unauthorized();
                }
                await _reservationService.DeleteReservationAsync(id);
                _logger.Information("Successfully deleted reservation with ID {ReservationId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Reservation with ID {ReservationId} not found", id);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warning("Failed to delete reservation with ID {ReservationId}", id);
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting reservation with ID {ReservationId}", id);
                return StatusCode(500, ex.Message);
            }
        }
    }
}
