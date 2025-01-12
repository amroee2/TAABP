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
    [Route("api/FeaturedDeals")]
    [ApiController]
    [Authorize]
    public class FeaturedDealController : ControllerBase
    {
        private readonly IFeaturedDealService _featuredDealService;
        private readonly ILogger _logger;
        private readonly IValidator<FeatueredDealDto> _featuredDealValidator;
        public FeaturedDealController(IFeaturedDealService featuredDealService, IValidator<FeatueredDealDto> validator)
        {
            _featuredDealService = featuredDealService;
            _logger = Log.ForContext<FeaturedDealController>();
            _featuredDealValidator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeaturedDealsAsync()
        {
            _logger.Information("Fetching featured deals");
            try
            {
                var featuredDeals = await _featuredDealService.GetFeaturedDealsAsync();
                _logger.Information("Successfully fetched {FeaturedDealCount} featured deals", featuredDeals.Count);
                return Ok(featuredDeals);
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching featured deals");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeaturedDealByIdAsync(int id)
        {
            _logger.Information("Fetching featured deal with ID {FeaturedDealId}", id);
            try
            {
                var featuredDeal = await _featuredDealService.GetFeaturedDealByIdAsync(id);
                _logger.Information("Successfully fetched featured deal with ID {FeaturedDealId}", id);
                return Ok(featuredDeal);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Featured deal with ID {FeaturedDealId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching featured deal with ID {FeaturedDealId}", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPost("Room/{roomId}")]
        public async Task<IActionResult> CreateFeaturedDealAsync(int roomId, FeatueredDealDto featuredDealDto)
        {
            _logger.Information("Creating featured deal for room with ID {RoomId}", roomId);
            try
            {
                await _featuredDealValidator.ValidateAndThrowAsync(featuredDealDto);
                featuredDealDto.RoomId = roomId;
                int id = await _featuredDealService.CreateFeaturedDealAsync(featuredDealDto);
                var featuredDeal = await _featuredDealService.GetFeaturedDealByIdAsync(id);
                _logger.Information("Successfully created featured deal with ID {FeaturedDealId} for room with ID {RoomId}", id, roomId);
                return StatusCode(201, featuredDeal);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Room with ID {RoomId} not found", roomId);
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while creating featured deal for room with ID {RoomId}", roomId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeaturedDealAsync(int id, FeatueredDealDto featuredDealDto)
        {
            _logger.Information("Updating featured deal with ID {FeaturedDealId}", id);
            try
            {
                await _featuredDealValidator.ValidateAndThrowAsync(featuredDealDto);
                featuredDealDto.FeaturedDealId = id;
                await _featuredDealService.UpdateFeaturedDealAsync(featuredDealDto);
                _logger.Information("Successfully updated featured deal with ID {FeaturedDealId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Featured deal with ID {FeaturedDealId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while updating featured deal with ID {FeaturedDealId}", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeaturedDealAsync(int id)
        {
            _logger.Information("Deleting featured deal with ID {FeaturedDealId}", id);
            try
            {
                await _featuredDealService.DeleteFeaturedDealAsync(id);
                _logger.Information("Successfully deleted featured deal with ID {FeaturedDealId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Featured deal with ID {FeaturedDealId} not found", id);
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while deleting featured deal with ID {FeaturedDealId}", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
