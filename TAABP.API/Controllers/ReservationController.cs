using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/User/{userId}/Room/{roomId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservationDto>>> GetReservations()
        {
            return Ok(await _reservationService.GetReservationsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            try
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);
                return Ok(reservation);
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
        public async Task<ActionResult> CreateReservation(string userId, int roomId, ReservationDto reservationDto)
        {
            reservationDto.UserId = userId;
            reservationDto.RoomId = roomId;
            try
            {
                var reservationId = await _reservationService.CreateReservationAsync(reservationDto);
                var reservation = await _reservationService.GetReservationByIdAsync(reservationId);
                return StatusCode(201, reservation);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{reservationId}")]
        public async Task<ActionResult> UpdateReservation(int reservationId, string userId, int roomId, ReservationDto reservationDto)
        {
            reservationDto.ReservationId = reservationId;
            reservationDto.UserId = userId;
            reservationDto.RoomId = roomId;
            try
            {
                await _reservationService.UpdateReservationAsync(reservationDto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReservation(int id)
        {
            try
            {
                await _reservationService.DeleteReservationAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
