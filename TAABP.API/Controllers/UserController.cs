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
    [Route("api/Users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly ICartItemService _cartItemService;
        private readonly ILogger _logger;
        private readonly IValidator<UserDto> _userValidator;
        public UserController(IUserService userService,
            IReviewService reviewService, ICartItemService cartItemService,
            IValidator<UserDto> validator)
        {
            _userService = userService;
            _reviewService = reviewService;
            _cartItemService = cartItemService;
            _logger = Log.ForContext<UserController>();
            _userValidator = validator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            _logger.Information("Fetching user with ID {UserId}", id);
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                _logger.Information("Successfully fetched user with ID {UserId}", id);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching user with ID {UserId}", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersAsync()
        {
            _logger.Information("Fetching all users");
            try
            {
                var users = await _userService.GetUsersAsync();
                _logger.Information("Successfully fetched all users");
                return Ok(users);
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching all users");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            _logger.Information("Deleting user with ID {UserId}", id);
            try
            {
                if(id != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to delete user with ID {UserId}", id);
                    return Unauthorized();
                }
                await _userService.DeleteUserAsync(id);
                _logger.Information("Successfully deleted user with ID {UserId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while deleting user with ID {UserId}", id);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] UserDto userDto)
        {
            _logger.Information("Updating user with ID {UserId}", id);
            try
            {
                if (id != _userService.GetCurrentUserId())
                {
                    _logger.Warning("Unauthorized access to update user with ID {UserId}", id);
                    return Unauthorized();
                }
                await _userValidator.ValidateAndThrowAsync(userDto);
                await _userService.UpdateUserAsync(id, userDto);
                _logger.Information("Successfully updated user with ID {UserId}", id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while updating user with ID {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("HotelsVisited")]
        public async Task<IActionResult> GetLastHotelsVisitedAsync()
        {
            string userId = _userService.GetCurrentUserId();
            _logger.Information("Fetching last hotels visited by user with ID {UserId}", userId);
            try
            {
                var hotels = await _userService.GetLastHotelsVisitedAsync(userId);
                _logger.Information("Successfully fetched last hotels visited by user with ID {UserId}", userId);
                return Ok(hotels);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", userId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching last hotels visited by user with ID {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{userId}/Reviews")]
        public async Task<IActionResult> GetAllUserReviewsAsync(string userId)
        {
            _logger.Information("Fetching all reviews made by user with ID {UserId}", userId);
            try
            {
                var reviews = await _reviewService.GetAllUserReviewsAsync(userId);
                _logger.Information("Successfully fetched all reviews made by user with ID {UserId}", userId);
                return Ok(reviews);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", userId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching all reviews made by user with ID {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("Carts")]
        public async Task<IActionResult> GetUserCartsAsync()
        {
            string userId = _userService.GetCurrentUserId();
            _logger.Information("Fetching carts for user with ID {UserId}", userId);
            try
            {
                var carts = await _cartItemService.GetUserCartsAsync(userId);
                _logger.Information("Successfully fetched carts for user with ID {UserId}", userId);
                return Ok(carts);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("User with ID {UserId} not found", userId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                _logger.Error("An error occurred while fetching carts for user with ID {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
