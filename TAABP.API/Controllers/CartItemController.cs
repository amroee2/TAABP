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
    [Route("api/Users/Carts")]
    [ApiController]
    [Authorize]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly IValidator<CartItemDto> _cartItemDtoValidator;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IUserService _userService;
        public CartItemController(
            IEmailService emailService,
            ICartItemService cartItemService,
            UserManager<User> userManager,
            IValidator<CartItemDto> cartItemDtoValidator,
            IPaymentMethodService paymentMethodService,
            IUserService userService)
        {
            _cartItemService = cartItemService;
            _emailService = emailService;
            _userManager = userManager;
            _logger = Log.ForContext<CartItemController>();
            _cartItemDtoValidator = cartItemDtoValidator;
            _paymentMethodService = paymentMethodService;
            _userService = userService;
        }

        [HttpPost("Rooms/{roomId}/CartItems")]
        public async Task<IActionResult> AddCartItemAsync( int roomId, CartItemDto cartItem)
        {
            _logger.Information("Adding cart item to cart");
            try
            {
                string userId = _userService.GetCurrentUserId();
                await _cartItemDtoValidator.ValidateAndThrowAsync(cartItem);
                cartItem.RoomId = roomId;
                var newCartItem = await _cartItemService.AddCartItemAsync(userId, cartItem);
                _logger.Information("Successfully added cart item with ID {CartItemId}", newCartItem.CartItemId);
                return StatusCode(201, newCartItem);
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart with ID {CartId} not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding cart item");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("CartItems/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItemAsync( int cartItemId)
        {
            _logger.Information("Deleting cart item with ID {CartItemId}}", cartItemId);
            try
            {
                string userId = _userService.GetCurrentUserId();

                await _cartItemService.DeleteCartItemAsync(userId, cartItemId);
                _logger.Information("Successfully deleted cart item with ID {CartItemId", cartItemId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart item with ID {CartItemId} not found", cartItemId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while deleting cart item with ID {CartItemId}", cartItemId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{cartId}/CartItems/{cartItemId}")]
        public async Task<IActionResult> GetCartItemByIdAsync(int cartId, int cartItemId)
        {
            _logger.Information("Fetching cart item with ID {CartItemId} from cart with ID {CartId}", cartItemId, cartId);
            try
            {
                string userId = _userService.GetCurrentUserId();

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

        [HttpGet("{cartId}/CartItems")]
        public async Task<IActionResult> GetCartItemsByCartIdAsync(int cartId)
        {
            _logger.Information("Fetching cart items from cart with ID {CartId}", cartId);
            try
            {
                string userId = _userService.GetCurrentUserId();

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

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartAsync(int cartId)
        {
            _logger.Information("Fetching cart with ID {CartId}", cartId);
            try
            {
                string userId = _userService.GetCurrentUserId();
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
        public async Task<IActionResult> ConfirmCartAsync(int paymentMethodId)
        {

            try
            {
                string userId = _userService.GetCurrentUserId();

                await _cartItemService.ConfirmCartAsync(userId, paymentMethodId);
                var user = await _userManager.FindByIdAsync(userId);
                await _emailService.SendEmailAsync(user.Email, "Cart Confirmed", "Your cart has been confirmed.");
                _logger.Information("Successfully confirmed cart with ID {CartId} using payment method with ID {PaymentMethodId}",  paymentMethodId);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.Warning("Cart with ID {CartId} not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while confirming cart with ID using payment method with ID {PaymentMethodId}", paymentMethodId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
