using TAABP.Application.DTOs.ShoppingDto;
using TAABP.Core.ShoppingEntities;

namespace TAABP.Application.ServiceInterfaces
{
    public interface ICartItemService
    {
        Task<CartItem> GetCartItemByIdAsync(int cartId, int cartItemId);
        Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task<CartItemDto> AddCartItemAsync(CartItemDto cartItem);
        Task DeleteCartItemAsync(int cartId, int cartItemId);
        Task<Cart> GetCartAsync(int cartId);
        Task ConfirmCartAsync(int cartId, int paymentMethodId);
        Task<List<Cart>> GetUserCartsAsync(string userId);
    }
}
