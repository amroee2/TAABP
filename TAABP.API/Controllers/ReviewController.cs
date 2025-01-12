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
    [Route("api/Users/{userId}/Hotels/{hotelId}/Reviews")]
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger _logger;
        private readonly IValidator<ReviewDto> _reviewValidator;
        private readonly IUserService _userService;
        public ReviewController(IReviewService reviewService, IValidator<ReviewDto> validator, IUserService userService)
        {
            _reviewService = reviewService;
            _logger = Log.ForContext<ReviewController>();
            _reviewValidator = validator;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReviewAsync(string userId, int hotelId, ReviewDto reviewDto)
        {
            _logger.Information("Adding review for hotel with ID {HotelId}", hotelId);
            try
            {
                if(userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to add review for hotel with ID {HotelId}", hotelId);
                    return Unauthorized();
                }
                await _reviewValidator.ValidateAndThrowAsync(reviewDto);
                reviewDto.UserId = userId;
                reviewDto.HotelId = hotelId;
                var reviewId = await _reviewService.AddReviewAsync(reviewDto);
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                _logger.Information("Successfully added review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                return StatusCode(201, review);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Hotel with ID {HotelId} not found", hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding review for hotel with ID {HotelId}", hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReviewAsync(int reviewId, int hotelId, string userId, ReviewDto reviewDto)
        {
            _logger.Information("Updating review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);

            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to update review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                    return Unauthorized();
                }
                await _reviewValidator.ValidateAndThrowAsync(reviewDto);
                reviewDto.ReviewId = reviewId;
                reviewDto.HotelId = hotelId;
                reviewDto.UserId = userId;
                await _reviewService.UpdateReviewAsync(reviewDto);
                _logger.Information("Successfully updated review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Review with ID {ReviewId} not found", reviewId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReviewAsync(int reviewId, int hotelId, string userId)
        {
            _logger.Information("Deleting review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
            try
            {
                if (userId != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to delete review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                    return Unauthorized();
                }
                await _reviewService.DeleteReviewAsync(userId, hotelId, reviewId);
                _logger.Information("Successfully deleted review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Review with ID {ReviewId} for hotel with ID {HotelId} not found", reviewId, hotelId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting review with ID {ReviewId} for hotel with ID {HotelId}", reviewId, hotelId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewByIdAsync(int reviewId)
        {
            _logger.Information("Getting review with ID {ReviewId}", reviewId);
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                _logger.Information("Successfully retrieved review with ID {ReviewId}", reviewId);
                return Ok(review);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Review with ID {ReviewId} not found", reviewId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while getting review with ID {ReviewId}", reviewId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
