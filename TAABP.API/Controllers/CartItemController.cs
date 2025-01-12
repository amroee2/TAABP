using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TAABP.Application;
using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;
using TAABP.Core;
using ILogger = Serilog.ILogger;

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
        private readonly ILogger _logger;
        private readonly IValidator<CartItemDto> _cartItemDtoValidator;
        public CartItemController(IEmailService emailService,
            ICartItemService cartItemService, UserManager<User> userManager,
            IValidator<CartItemDto> cartItemDtoValidator)
        {
            _cartItemService = cartItemService;
            _emailService = emailService;
            _userManager = userManager;
            _logger = Log.ForContext<CartItemController>();
            _cartItemDtoValidator = cartItemDtoValidator;
        }

        [HttpPost("Rooms/{roomId}/CartItems")]
        public async Task<IActionResult> AddCartItemAsync(int cartId, int roomId, CartItemDto cartItem)
        {
            _logger.Information("Adding cart item to cart with ID {CartId}", cartId);
            try
            {
                await _cartItemDtoValidator.ValidateAndThrowAsync(cartItem);
                cartItem.CartId = cartId;
                cartItem.RoomId = roomId;
                var newCartItem = await _cartItemService.AddCartItemAsync(cartItem);
                _logger.Information("Successfully added cart item with ID {CartItemId} to cart with ID {CartId}", newCartItem.CartItemId, cartId);
                return StatusCode(201, newCartItem);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart with ID {CartId} not found", cartId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding cart item to cart with ID {CartId}", cartId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("CartItems/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItemAsync(int cartId, int cartItemId)
        {
            _logger.Information("Deleting cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
            try
            {
                await _cartItemService.DeleteCartItemAsync(cartId, cartItemId);
                _logger.Information("Successfully deleted cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart item with ID {CartItemId} not found in cart with ID {CartId}", cartItemId, cartId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("CartItems/{cartItemId}")]
        public async Task<IActionResult> GetCartItemByIdAsync(int cartId, int cartItemId)
        {
            _logger.Information("Fetching cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
            try
            {
                var cartItem = await _cartItemService.GetCartItemByIdAsync(cartId, cartItemId);
                _logger.Information("Successfully fetched cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
                return Ok(cartItem);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart item with ID {CartItemId} not found in cart with ID {CartId}", cartItemId, cartId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("CartItems")]
        public async Task<IActionResult> GetCartItemsByCartIdAsync(int cartId)
        {
            _logger.Information("Fetching cart items from cart with ID {CartId}", cartId);
            try
            {
                var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cartId);
                _logger.Information("Successfully fetched cart items from cart with ID {CartId}", cartId);
                return Ok(cartItems);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart with ID {CartId} not found", cartId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching cart items from cart with ID {CartId}", cartId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartAsync(int cartId)
        {
            _logger.Information("Fetching cart with ID {CartId}", cartId);
            try
            {
                var cart = await _cartItemService.GetCartAsync(cartId);
                _logger.Information("Successfully fetched cart with ID {CartId}", cartId);
                return Ok(cart);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart with ID {CartId} not found", cartId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while fetching cart with ID {CartId}", cartId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("Payment/{paymentMethodId}/Confirm")]
        public async Task<IActionResult> ConfirmCartAsync(int cartId, int paymentMethodId)
        {
            _logger.Information("Confirming cart with ID {CartId} using payment method with ID {PaymentMethodId}", cartId, paymentMethodId);
            try
            {
                string userId = await _cartItemService.ConfirmCartAsync(cartId, paymentMethodId);
                var user = await _userManager.FindByIdAsync(userId);
                await _emailService.SendEmailAsync(user.Email, "Cart Confirmed", "Your cart has been confirmed.");
                _logger.Information("Successfully confirmed cart with ID {CartId} using payment method with ID {PaymentMethodId}", cartId, paymentMethodId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart with ID {CartId} not found", cartId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while confirming cart with ID {CartId} using payment method with ID {PaymentMethodId}", cartId, paymentMethodId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
