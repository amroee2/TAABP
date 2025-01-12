using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs;
using TAABP.Application.Exceptions;
using TAABP.Application.Profile.UserMapping;
using TAABP.Application.ServiceInterfaces;

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
        public UserController(IUserService userService,
            IReviewService reviewService, ICartItemService cartItemService)
        {
            _userService = userService;
            _reviewService = reviewService;
            _cartItemService = cartItemService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] UserDto userDto)
        {
            try
            {
                await _userService.UpdateUserAsync(id, userDto);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchUserAsync(string id, JsonPatchDocument<UserDto> patchDoc)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                patchDoc.ApplyTo(user, (error) => ModelState.AddModelError(error.AffectedObject.ToString(), error.ErrorMessage));

                if (!TryValidateModel(user))
                {
                    return ValidationProblem(ModelState);
                }

                await _userService.UpdateUserAsync(id, user);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{userId}/HotelsVisited")]
        public async Task<IActionResult> GetLastHotelsVisitedAsync(string userId)
        {
            try
            {
                var hotels = await _userService.GetLastHotelsVisitedAsync(userId);
                return Ok(hotels);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{userId}/Reviews")]
        public async Task<IActionResult> GetAllUserReviewsAsync(string userId)
        {
            try
            {
                var reviews = await _reviewService.GetAllUserReviewsAsync(userId);
                return Ok(reviews);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        [HttpGet("{userId}/Carts")]
        public async Task<IActionResult> GetUserCartsAsync(string userId)
        {
            try
            {
                var carts = await _cartItemService.GetUserCartsAsync(userId);
                return Ok(carts);
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
