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
    [Route("api/Users/Reservations")]
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
        public async Task<ActionResult<List<ReservationDto>>> GetReservations()
        {
            _logger.Information("Fetching all reservations");
            try
            {
                var userId = _userService.GetCurrentUserId();
                var reservations = await _reservationService.GetReservationsAsync(userId);
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
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            _logger.Information("Fetching reservation with ID {ReservationId}", id);
            try
            {
                var userId = _userService.GetCurrentUserId();
                var reservation = await _reservationService.GetReservationByIdAsync(userId, id);
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

        [HttpPut("{reservationId}")]
        public async Task<ActionResult> UpdateReservation(int reservationId, ReservationDto reservationDto)
        {
            var userId = _userService.GetCurrentUserId();
            _logger.Information("Updating reservation with ID {ReservationId} for user with ID {UserId}", reservationId, userId);
            try
            {
                await _reservationValidator.ValidateAndThrowAsync(reservationDto);
                reservationDto.ReservationId = reservationId;
                reservationDto.UserId = userId;
                await _reservationService.UpdateReservationAsync(reservationDto);
                _logger.Information("Successfully updated reservation with ID {ReservationId} for user with ID {UserId} ", reservationId, userId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Reservation with ID {ReservationId} for user with ID {UserId}", reservationId, userId);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warning("Failed to update reservation with ID {ReservationId} for user with ID {UserId} ", reservationId, userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating reservation with ID {ReservationId} for user with ID {UserId}", reservationId, userId);
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            var userId = _userService.GetCurrentUserId();
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
