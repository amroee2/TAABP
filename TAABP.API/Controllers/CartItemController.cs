using Microsoft.AspNetCore.Mvc;
using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Application.Exceptions;
using TAABP.Application.ServiceInterfaces;

namespace TAABP.API.Controllers
{
    [Route("api/Carts/{cartId}/Rooms/{roomId}/CartItems")]
    [ApiController]
    public class CartItemController : ControllerBase
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet("{cartItemId}")]
        public async Task<IActionResult> GetCartItemByIdAsync(int cartItemId)
        {
            try
            {
                var cartItem = await _cartItemService.GetCartItemByIdAsync(cartItemId);
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

        [HttpPost]
        public async Task<IActionResult> AddCartItemAsync(int cartId, int roomId, CartItemDto cartItem)
        {
            try
            {
                cartItem.CartId = cartId;
                cartItem.RoomId = roomId;
                int cartItemId = await _cartItemService.AddCartItemAsync(cartItem);
                var newCartItem = await _cartItemService.GetCartItemByIdAsync(cartItemId);
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

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItemAsync(int cartItemId)
        {
            try
            {
                await _cartItemService.DeleteCartItemAsync(cartItemId);
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
