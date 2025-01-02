using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/User/{userId}/Hotel/{hotelId}/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReviewAsync(string userId, int hotelId, ReviewDto reviewDto)
        {
            try
            {
                reviewDto.UserId = userId;
                reviewDto.HotelId = hotelId;
                var reviewId = await _reviewService.AddReviewAsync(reviewDto);
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                return StatusCode(201, review);
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

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReviewAsync(int reviewId, int hotelId, string userId, ReviewDto reviewDto)
        {

            try
            {
                reviewDto.ReviewId = reviewId;
                reviewDto.HotelId = hotelId;
                reviewDto.UserId = userId;
                await _reviewService.UpdateReviewAsync(reviewDto);
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

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReviewAsync(int reviewId, int hotelId, string userId)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(userId, hotelId, reviewId);
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

        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReviewByIdAsync(int reviewId)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(reviewId);
                return Ok(review);
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
