using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TAABP.Application;
using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;

namespace TAABP.API.Controllers
{
    [Route("api/Carts/{cartId}")]
    [ApiController]
    [Authorize]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        public CartItemController(IEmailService emailService, 
            ICartItemService cartItemService, UserManager<User> userManager)
        {
            _cartItemService = cartItemService;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpPost("Rooms/{roomId}/CartItems")]
        public async Task<IActionResult> AddCartItemAsync(int cartId, int roomId, CartItemDto cartItem)
        {
            try
            {
                cartItem.CartId = cartId;
                cartItem.RoomId = roomId;
                var newCartItem = await _cartItemService.AddCartItemAsync(cartItem);
                return StatusCode(201, newCartItem);
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

        [HttpDelete("CartItems/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItemAsync(int cartId, int cartItemId)
        {
            try
            {
                await _cartItemService.DeleteCartItemAsync(cartId, cartItemId);
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

        [HttpGet("CartItems/{cartItemId}")]
        public async Task<IActionResult> GetCartItemByIdAsync(int cartId, int cartItemId)
        {
            try
            {
                var cartItem = await _cartItemService.GetCartItemByIdAsync(cartId, cartItemId);
                return Ok(cartItem);
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

        [HttpGet("CartItems")]
        public async Task<IActionResult> GetCartItemsByCartIdAsync(int cartId)
        {
            try
            {
                var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cartId);
                return Ok(cartItems);
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

        [HttpGet]
        public async Task<IActionResult> GetCartAsync(int cartId)
        {
            try
            {
                var cart = await _cartItemService.GetCartAsync(cartId);
                return Ok(cart);
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

        [HttpPost("Payment/{paymentMethodId}/Confirm")]
        public async Task<IActionResult> ConfirmCartAsync(int cartId, int paymentMethodId)
        {
            try
            {
                string userId = await _cartItemService.ConfirmCartAsync(cartId, paymentMethodId);
                var user = await _userManager.FindByIdAsync(userId);
                await _emailService.SendEmailAsync(user.Email, "Cart Confirmed", "Your cart has been confirmed.");
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
    }
}
